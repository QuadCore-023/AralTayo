#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

// Requiring 'Graphic' allows this script to work flawlessly with BOTH standard Image and RawImage components!
[RequireComponent(typeof(Graphic))]
[RequireComponent(typeof(RectTransform))]
public class CompanionController : MonoBehaviour
{
    public enum CompanionType { Land, Flying, Bouncy }
    public enum FlyingType { Wing, Floating }

    [Header("Companion Identity")]
    public CompanionType currentType = CompanionType.Land;
    public FlyingType flyingStyle = FlyingType.Wing;

    [Header("Movement Settings")]
    [Tooltip("How fast the companion walks across the floor")]
    public float walkSpeed = 150f;
    [Tooltip("How fast the companion moves when in the air")]
    public float flySpeed = 200f;
    [Tooltip("Min and Max time the companion stays still before moving again")]
    public Vector2 idleTimeRange = new Vector2(1f, 4f);

    [Header("Floor Boundaries (UI Local Positions)")]
    [Tooltip("The lowest and furthest left the companion can walk")]
    public Vector2 floorMinBounds = new Vector2(-300f, -400f);
    [Tooltip("The highest and furthest right the companion can walk")]
    public Vector2 floorMaxBounds = new Vector2(300f, -200f);

    [Header("Room Restrictions")]
    [Tooltip("Drag the BedroomScreen GameObject here")]
    public GameObject bedroomScreen;
    [Tooltip("Drag the DiningRoomScreen GameObject here")]
    public GameObject diningRoomScreen;
    [Tooltip("Drag the HallwayScreen GameObject here")]
    public GameObject hallwayScreen;

    // Internal State Machine
    private enum MovementState { Idling, Walking, Flying, Bouncing }
    private MovementState currentState = MovementState.Idling;

    private RectTransform rectTransform;
    private Graphic companionGraphic; // Dynamically handles Image or RawImage
    private Vector2 targetPosition;
    private float stateTimer;
    private bool isFacingRight = true;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        companionGraphic = GetComponent<Graphic>();

        // Force UI anchors to a single center point to prevent stretching issues
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    private void Start()
    {
        PickNewTargetPosition();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = targetPosition;
        }
        currentState = MovementState.Walking;
    }

    private void Update()
    {
        // 1. Manage overall visibility based on what screen is active
        HandleVisibility();

        // Safety Check: If the graphic component is missing or disabled, stop tracking movement
        if (companionGraphic == null || !companionGraphic.enabled) return;

        // 2. Handle State Machine based on companion type
        switch (currentType)
        {
            case CompanionType.Land:
                HandleLandMovement();
                break;

            case CompanionType.Flying:
                HandleFutureFlyingMovement();
                break;

            case CompanionType.Bouncy:
                HandleFutureBouncyMovement();
                break;
        }
    }

    /// <summary>
    /// Checks the active hierarchy state of allowed rooms to show or hide the companion asset.
    /// </summary>
    private void HandleVisibility()
    {
        if (companionGraphic == null || rectTransform == null) return;

        bool isInValidRoom = (bedroomScreen != null && bedroomScreen.activeInHierarchy) ||
                             (diningRoomScreen != null && diningRoomScreen.activeInHierarchy) ||
                             (hallwayScreen != null && hallwayScreen.activeInHierarchy);

        if (companionGraphic.enabled != isInValidRoom)
        {
            companionGraphic.enabled = isInValidRoom;
            if (isInValidRoom)
            {
                PickNewTargetPosition();
                rectTransform.anchoredPosition = targetPosition;
                currentState = MovementState.Walking;
            }
        }
    }

    /// <summary>
    /// Core logic driving the simple horizontal/floor wandering behavior.
    /// </summary>
    private void HandleLandMovement()
    {
        if (currentState == MovementState.Idling)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                PickNewTargetPosition();
                currentState = MovementState.Walking;
            }
        }
        else if (currentState == MovementState.Walking)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, walkSpeed * Time.deltaTime);
            FlipSpriteTowardsTarget(targetPosition.x - rectTransform.anchoredPosition.x);

            if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 2f)
            {
                currentState = MovementState.Idling;
                stateTimer = Random.Range(idleTimeRange.x, idleTimeRange.y);
            }
        }
    }

    private void PickNewTargetPosition()
    {
        if (Vector2.Distance(floorMinBounds, floorMaxBounds) < 5f)
        {
            floorMinBounds = new Vector2(-200f, -300f);
            floorMaxBounds = new Vector2(200f, -100f);
        }

        float randomX = Random.Range(floorMinBounds.x, floorMaxBounds.x);
        float randomY = Random.Range(floorMinBounds.y, floorMaxBounds.y);
        targetPosition = new Vector2(randomX, randomY);
    }

    private void FlipSpriteTowardsTarget(float directionX)
    {
        if (directionX > 0.1f && !isFacingRight)
        {
            isFacingRight = true;
            rectTransform.localScale = new Vector3(Mathf.Abs(rectTransform.localScale.x), rectTransform.localScale.y, rectTransform.localScale.z);
        }
        else if (directionX < -0.1f && isFacingRight)
        {
            isFacingRight = false;
            rectTransform.localScale = new Vector3(-Mathf.Abs(rectTransform.localScale.x), rectTransform.localScale.y, rectTransform.localScale.z);
        }
    }

    private void HandleFutureFlyingMovement()
    {
        HandleLandMovement();
    }

    private void HandleFutureBouncyMovement()
    {
        HandleLandMovement();
    }
}

// ------------------------------------------------------------------------
// CUSTOM INSPECTOR LAYOUT
// ------------------------------------------------------------------------
#if UNITY_EDITOR
[CustomEditor(typeof(CompanionController))]
public class CompanionControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw Identity Selection
        EditorGUILayout.LabelField("Companion Identity", EditorStyles.boldLabel);
        SerializedProperty currentTypeProp = serializedObject.FindProperty("currentType");
        EditorGUILayout.PropertyField(currentTypeProp);

        if (currentTypeProp.enumValueIndex == (int)CompanionController.CompanionType.Flying)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flyingStyle"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Draw Movement Values Layout
        EditorGUILayout.LabelField("Movement Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("walkSpeed"));

        if (currentTypeProp.enumValueIndex == (int)CompanionController.CompanionType.Flying)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flySpeed"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("idleTimeRange"));

        EditorGUILayout.Space();

        // Draw Position Constraints
        EditorGUILayout.LabelField("Floor Boundaries (UI Local Positions)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("floorMinBounds"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("floorMaxBounds"));

        EditorGUILayout.Space();

        // Draw Scene Management System Target Links
        EditorGUILayout.LabelField("Room Restrictions", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bedroomScreen"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("diningRoomScreen"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hallwayScreen"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif