using System.Collections;
using UnityEngine;

public class AnimateTextMesh : MonoBehaviour {
    public TextMesh textMesh;
    public float animSpeedInSec = 0.5f;
    private bool keepAnimating = false;

    // Use this for initialization
    private void Start() {
        startTextMeshAnimation();
    }

    private IEnumerator anim() {
        Color currentColor = textMesh.color;

        Color invisibleColor = textMesh.color;
        invisibleColor.a = 0.5f; //Set Alpha to 0

        float oldAnimSpeedInSec = animSpeedInSec;
        float counter = 0;
        while (keepAnimating) {
            //Hide Slowly
            while (counter < oldAnimSpeedInSec) {
                if (!keepAnimating) {
                    yield break;
                }

                //Reset counter when Speed is changed
                if (oldAnimSpeedInSec != animSpeedInSec) {
                    counter = 0;
                    oldAnimSpeedInSec = animSpeedInSec;
                }

                counter += Time.deltaTime;
                textMesh.color = Color.Lerp(currentColor, invisibleColor, counter / oldAnimSpeedInSec);
                yield return null;
            }

            yield return null;

            //Show Slowly
            while (counter > 0) {
                if (!keepAnimating) {
                    yield break;
                }

                //Reset counter when Speed is changed
                if (oldAnimSpeedInSec != animSpeedInSec) {
                    counter = 0;
                    oldAnimSpeedInSec = animSpeedInSec;
                }

                counter -= Time.deltaTime;
                textMesh.color = Color.Lerp(currentColor, invisibleColor, counter / oldAnimSpeedInSec);
                yield return null;
            }
        }
    }

    //Call to Start animation
    private void startTextMeshAnimation() {
        if (keepAnimating) {
            return;
        }
        keepAnimating = true;
        StartCoroutine(anim());
    }

    //Call to Change animation Speed
    private void changeTextMeshAnimationSpeed(float animSpeed) {
        animSpeedInSec = animSpeed;
    }

    //Call to Stop animation
    private void stopTextMeshAnimation() {
        keepAnimating = false;
    }
}