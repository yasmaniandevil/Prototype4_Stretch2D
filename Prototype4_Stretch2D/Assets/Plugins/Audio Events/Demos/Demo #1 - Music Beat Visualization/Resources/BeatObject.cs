namespace Dimension19.AudioEvents.DemoScenes
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class BeatObject : MonoBehaviour
    {
        public Vector3 startScale = Vector3.one;
        public Vector3 endScale = new Vector3(1.1f, 1.1f, 1.1f);

        public float startDuration = 0.025f;
        private float startElapsedTime = 0f;

        public float endDuration = 0.05f;
        private float endElapsedTime = 0f;

        public void OnBeat()
        {
            //Debug.Log("OnBeat() Invoked!");
            StartCoroutine("OnBeatUpdate");
        }

        private IEnumerator OnBeatUpdate()
        {
            while (startElapsedTime < startDuration)
            {
                this.gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, (startElapsedTime / startDuration));
                startElapsedTime += Time.deltaTime;

                yield return null;
            }

            yield return null;

            while (endElapsedTime < endDuration)
            {
                this.gameObject.transform.localScale = Vector3.Lerp(endScale, startScale, (endElapsedTime / endDuration));
                endElapsedTime += Time.deltaTime;

                yield return null;
            }

            this.gameObject.transform.localScale = startScale;
            startElapsedTime = 0f;
            endElapsedTime = 0f;
        }
    }
}