using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmolAme.Trainer.Components.Hitboxes;

public class HitboxRenderer : MonoBehaviour {
    private static readonly Material material = new(Shader.Find("Sprites/Default")) {
        renderQueue = 4000
    };

    private Dictionary<Collider2D, LineRenderer> colliders = new();
    private bool inCamera = true;

    private void Start() {
        SetupColliders();
    }

    private void LateUpdate() {
        if (!HitboxController.Show && enabled) {
            SetEnabled(false);
            return;
        }

        if (Camera.main != null) {
            inCamera = Vector2.Distance(transform.position, Camera.main.transform.position) < 40;
        }

        if (inCamera) {
            UpdateColliders();
        }
    }

    private Color GetColor(Collider2D collider2D) {
        // 0: Default
        // 1: TransparentFX
        // 2: Ignore Raycast
        // 4: Water
        // 5: UI
        // 8: Player
        // 9: Shadow
        // 10: Background
        // 11: Death
        // 12: Boss
        // 13: Handle
        // 14: Collectible
        // 15: Trigger
        // 16: TriggerSensor
        // 17: DepthReserve
        // 18: Transition
        GameObject go = collider2D.gameObject;
        int layer = go.layer;
        if (layer == 8) {
            return Color.green;
        } else if (layer is 15 or 16) {
            return new Color(0.5f, 0.5f, 1f);
        } else if (layer == 14) {
            return Color.yellow;
        } else if (layer == 11) {
            return Color.red;
        } else if (go.GetComponentInChildren<BouncyScript>() || go.GetComponentInParent<BouncyScript>()) {
            return Color.cyan;
        } else {
            return new Color(1f, 0.5f, 0.25f);
        }
    }

    private void SetupColliders() {
        Collider2D[] components = gameObject.GetComponents<Collider2D>();

        foreach (Collider2D collider in components) {
            if (!collider) {
                continue;
            }

            if (collider is not (BoxCollider2D or EdgeCollider2D or PolygonCollider2D or CircleCollider2D)) {
                continue;
            }

            LineRenderer renderer = new GameObject("Hitbox").AddComponent<LineRenderer>();
            colliders[collider] = renderer;
            renderer.gameObject.transform.SetParent(gameObject.transform, false);

            // make hitbox has the same z as the player
            Vector3 position = renderer.transform.position;
            renderer.transform.position = new Vector3(position.x, position.y, 0);

            SetupLineRenderer(renderer, collider);
        }
    }

    private Vector3[] GetPoints(Collider2D collider) {
        Vector2 offset = collider.offset;
        Vector3 offsetV3 = offset;

        switch (collider) {
            case BoxCollider2D box:
                Vector2 halfSize = box.size / 2;
                return new Vector3[4] {
                    new Vector3(-halfSize.x, -halfSize.y, 0f) + offsetV3,
                    new Vector3(-halfSize.x, halfSize.y, 0f) + offsetV3,
                    new Vector3(halfSize.x, halfSize.y, 0f) + offsetV3,
                    new Vector3(halfSize.x, -halfSize.y, 0f) + offsetV3
                };
            case EdgeCollider2D edge:
                return edge.points.Select(point => (Vector3) (point + offset)).ToArray();
            case PolygonCollider2D polygon:
                return polygon.points.Select(point => (Vector3) (point + offset)).ToArray();
            case CircleCollider2D circle:
                float radius = circle.radius;
                Vector2 center = circle.transform.TransformPoint(Vector3.zero);
                Vector2 right = circle.transform.TransformPoint(Vector3.right * radius);
                Vector2 up = circle.transform.TransformPoint(Vector3.up * radius);
                float worldSpaceRadius = (Vector2.Distance(center, right) + Vector2.Distance(center, up)) / 2f;
                int pointsLength = (int) (worldSpaceRadius * 32);
                float sliceSize = Mathf.PI * 2 / pointsLength;
                Vector3[] points = new Vector3[pointsLength];
                for (int i = 1; i < pointsLength + 1; i++) {
                    float theta = sliceSize * i;
                    points[i - 1] = new Vector3(Mathf.Sin(theta), Mathf.Cos(theta), 0f) * radius + offsetV3;
                }

                return points;
            default:
                return Array.Empty<Vector3>();
        }
    }

    private void SetupLineRenderer(LineRenderer renderer, Collider2D collider) {
        Vector3[] points = GetPoints(collider);
        renderer.material = material;
        renderer.positionCount = points.Length;
        renderer.SetPositions(points);
        renderer.startColor = GetColor(collider);
        renderer.endColor = GetColor(collider);
        renderer.widthMultiplier = 0.05f;
        renderer.useWorldSpace = false;
        renderer.loop = collider is not EdgeCollider2D;
        renderer.enabled = collider.isActiveAndEnabled && HitboxController.Show;
    }

    private void UpdateColliders() {
        foreach (Collider2D collider in colliders.Keys) {
            LineRenderer renderer = colliders[collider];

            renderer.enabled = collider.isActiveAndEnabled;
            if (!renderer.enabled) {
                continue;
            }

            renderer.SetPositions(GetPoints(collider));
        }
    }

    private void OnDestroy() {
        foreach (LineRenderer renderer in colliders.Values) {
            Destroy(renderer);
        }
    }

    public void SetEnabled(bool value) {
        enabled = value;

        foreach (LineRenderer renderer in colliders.Values) {
            renderer.enabled = value;
        }
    }
}