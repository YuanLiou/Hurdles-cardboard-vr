using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour {

    public BezierSpline spline;
    public SplineWalkerMode mode;
    public float duration;
    public bool lookForward;
    private float progress;
    private bool goingForward;

    // Use this for initialization
    void Start() { }

    // Update is called once per frame
    void Update() {
        if (goingForward) {
            progress += Time.deltaTime / duration;
            if (progress > 1f) {
                if (mode == SplineWalkerMode.Once) {
                    progress = 1f;
                } else if (mode == SplineWalkerMode.Loop) {
                    progress -= 1f;
                } else {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        } else {
            progress -= Time.deltaTime / duration;
            if (progress < 0f) {
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = new Vector3(position.x, transform.position.y, position.z);

        if (lookForward) {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}