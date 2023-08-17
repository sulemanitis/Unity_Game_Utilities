using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GameUtilities
{
    public static class Helpers
    {
        private static readonly System.Random rng = new();

        private static Matrix4x4 isoMatrix;

        public static Vector3 ToIso(this Vector3 input, float cameraY = 45)
        {
            isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, cameraY, 0));
            return isoMatrix.MultiplyPoint3x4(input);
        }

        public static bool ReachedDestinationOrGaveUp(this NavMeshAgent navMeshAgent)
        {
            if (!navMeshAgent.pathPending
                && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
                && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f))
                return true;
            else
                return false;
        }
        public static void SetTextWithSeparator(this TMP_Text textMesh, int text, string prefix = "")
        {
            textMesh.text = prefix + string.Format("{0:N0}", text);
        }

        public static void SetTextWithSeparator(this TMP_Text textMesh, float text, string prefix = "")
        {
            textMesh.text = prefix + string.Format("{0:N0}", text);
        }

        public static IEnumerator TextCounter(TMP_Text textMesh, int startValue, int endValue, float duration, string prefix = "", System.Action OnComplete = null)
        {

            duration = Mathf.Clamp(duration, 0.1f, 5f);
            bool isIncrement = startValue < endValue;


            int counterSteps = (int)(duration / 0.02f);
            int fraction = isIncrement ? (endValue - startValue) / counterSteps : (startValue - endValue) / counterSteps;

            for (int i = 0; i < counterSteps; i++)
            {
                if (isIncrement)
                    startValue += fraction;
                else
                    startValue -= fraction;

                textMesh.SetTextWithSeparator(startValue, prefix);
                if (i == counterSteps - 1)
                    textMesh.SetTextWithSeparator(endValue, prefix);

                yield return new WaitForSecondsRealtime(0.02f);
            }
            OnComplete?.Invoke();
        }



        public static void ShowPopup(this GameObject btn,float duration, Action onComplete = null)
        {
            btn.transform.DOKill();
            btn.transform.localScale = Vector3.zero;
            btn.SetActive(true);
            btn.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public static void HidePopup(this GameObject btn,float duration, Action onComplete = null)
        {
            btn.transform.DOKill();
            btn.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack).OnComplete(() =>
            {
                btn.SetActive(false);
                onComplete?.Invoke();
            });
        }

        public static T GetRandomEnumValue<T>()
        {
            System.Array enumValues = System.Enum.GetValues(typeof(T));
            return (T)enumValues.GetValue(Random.Range(0, enumValues.Length));
        }
        public static void Randomize<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static float ClampAngle(float angle, float from, float to)
        {
            // Normalize the angle within the range of -360 to 360
            angle %= 360;

            // Make sure the "from" angle is positive
            if (from < 0)
                from = 360 + from;

            // Make sure the "to" angle is positive
            if (to < 0)
                to = 360 + to;

            // Handle the case where "from" is greater than "to"
            if (from > to)
            {
                // Normalize the angle within the range of 0 to 360
                if (angle < 0)
                    angle = 360 + angle;

                // Check if the angle is within the range from "from" to 360 or from 0 to "to"
                if (angle >= from || angle <= to)
                    return angle;
                else
                    return (Mathf.Abs(angle - from) < Mathf.Abs(angle - to)) ? from : to;
            }
            else
            {
                // Clamp the angle within the range from "from" to "to"
                if (angle < from)
                    angle = from;
                else if (angle > to)
                    angle = to;

                return angle;
            }
        }
        public static bool IsNaN(this Vector3 vector)
        {
            if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
                return true;

            return false;
        }


        public static Coroutine DelayExecution(this MonoBehaviour invoker, float delayTime, Action beforeAction, Action afterAction)
        {
            return invoker.StartCoroutine(CO_DelayExecution());

            IEnumerator CO_DelayExecution()
            {
                beforeAction?.Invoke();
                yield return new WaitForSeconds(delayTime);
                afterAction?.Invoke();
            }
        }
        public static Coroutine DelayExecution(this MonoBehaviour invoker, float delayTime, Action action)
        {
            return invoker.StartCoroutine(CO_DelayExecution());

            IEnumerator CO_DelayExecution()
            {
                yield return new WaitForSeconds(delayTime);
                action?.Invoke();
            }
        }

        public static void SpawnCoins(Transform spawningCoin, int quantity, Transform coinEndPos, bool scaleAnimation = true)
        {

            const float disFromCenter = 20f;
            quantity = Mathf.Clamp(quantity, 1, 50);

            for (int i = 0; i < quantity; i++)
            {
                Transform coin = UnityEngine.Object.Instantiate(spawningCoin, spawningCoin.position, spawningCoin.rotation, spawningCoin.parent);

                Vector3 randOffset = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0f) + spawningCoin.localPosition;
                randOffset.x += Mathf.Sign(randOffset.x) * disFromCenter;
                randOffset.y += Mathf.Sign(randOffset.y) * disFromCenter;

                //if (scaleAnimation)
                //{
                //    float scaleDuration = Random.Range(0.3f, 0.6f);
                //    sequence.Append(coin.DOScale(coin.localScale / 1.75f, scaleDuration));
                //}

                Sequence sequence = DOTween.Sequence();
                sequence.Append(coin.DOLocalMove(randOffset, 0.5f))
                .Append(coin.DOMove(coinEndPos.position, (float)Random.Range(0.7f, 1f)).SetEase(Ease.InBack).SetDelay(i * 0.02f))
                .Append(coin.DOScale(0, 0.5f)).SetEase(Ease.Linear).SetDelay(i * 0.02f)
                .OnComplete(() => UnityEngine.Object.Destroy(coin.gameObject, 1f));
            }
        }

        public static GameObject FindChildByName(string name, GameObject parent)
        {
            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            GameObject child = null;

            for (int i = 0; i < childs.Length; i++)
            {
                if (childs[i].gameObject.name.Equals(name))
                {
                    child = childs[i].gameObject;
                    break;
                }
            }

            return child;
        }

        public static BGMConfig AddBGM(AudioClip audioClip)
        {
            GameObject bgmGameObject = new()
            {
                name = "BGM"
            };
            BGMConfig bGMConfig = bgmGameObject.AddComponent<BGMConfig>();
            bGMConfig.Configuration(audioClip);
            return bGMConfig;
        }

        public static void SpawnCoins(Transform refCoin, int quanitity, Transform coinEndPos, bool scaleAnimation = true, Action OnVibrate = null, Action OnComplete = null)
        {
            quanitity = Mathf.Clamp(quanitity, 1, 50);

            int index = 0;
            for (int i = 0; i < quanitity; i++)
            {
                index = i;
                Sequence sequence = DOTween.Sequence();
                Transform coin = UnityEngine.Object.Instantiate(refCoin, refCoin.position, refCoin.rotation, refCoin);
                coin.gameObject.SetActive(true);
                float randX = Random.Range(-50f, 50f);
                float randY = Random.Range(-50f, 50f);
                float disFromCenter = 20;
                if (randX > 0)
                    randX += disFromCenter;
                else if (randX < 0)
                    randX -= disFromCenter;

                if (randY > 0)
                    randY += disFromCenter;
                else if (randY < 0)
                    randY -= disFromCenter;

                sequence.Append(coin.DOLocalMove(new Vector3(randX, randY, 0), Random.Range(0.6f, 0.6f)));
                if (scaleAnimation)
                    sequence.Append(coin.DOScale(coin.localScale / 1.75f, Random.Range(0.3f, 0.6f)));
                sequence.Append(coin.DOMove(coinEndPos.position, Random.Range(0.7f, 1f)).SetEase(Ease.InBack)).OnComplete(() =>
                {
                    UnityEngine.Object.Destroy(coin.gameObject, 1);
                    OnVibrate?.Invoke();
                });
                if (index == quanitity - 1)
                {
                    OnComplete?.Invoke();
                }

            }

        }

        public static void SetText(TextMeshPro textMesh, int value)
        {
            if (value < 5000)
                textMesh.text = value.ToString("F0");
            else
            {
                float _value = value / (float)1000;
                textMesh.text = $"{_value:F2}K";
            }
        }
        public static void SetText(TextMeshProUGUI textMesh, int value)
        {
            if (value < 10000)
                textMesh.text = value.ToString("F0");
            else
            if (value >= 10000 && value < 1000000)
            {
                float _value = value / (float)1000;
                textMesh.text = $"{_value:F1}K";
            }
            else if (value >= 1000000)
            {
                float _value = value / (float)1000000;
                textMesh.text = $"{_value:F2}M";
            }
        }

        public static IEnumerator TextCounter(TextMeshProUGUI textMeshProUGUI, int startValue, int endValue, float duration, Action OnComplete = null)
        {

            duration = Mathf.Clamp(duration, 0.1f, 5f);
            bool isIncrement = startValue < endValue;
            int divider = 1;
            string suffix = "";
            int decimalPlaces = 0;

            if (endValue >= 10000 && endValue < 1000000)
            {
                divider = 1000;
                suffix = "K";
                decimalPlaces = 1;
            }
            else if (endValue >= 1000000)
            {
                divider = 1000000;
                suffix = "M";
                decimalPlaces = 2;
            }

            float _startValue = (float)startValue / divider;
            float _endValue = (float)endValue / divider;
            int counterSteps = (int)(duration / 0.02f);
            float fraction = isIncrement ? (_endValue - _startValue) / counterSteps : (_startValue - _endValue) / counterSteps;

            for (int i = 0; i < counterSteps; i++)
            {
                if (isIncrement)
                    _startValue += fraction;
                else
                    _startValue -= fraction;

                textMeshProUGUI.text = $"{_startValue.ToString("F" + decimalPlaces)}{suffix}";
                if (i == counterSteps - 1)
                    textMeshProUGUI.text = $"{_endValue.ToString("F" + decimalPlaces)}{suffix}";

                yield return new WaitForSecondsRealtime(0.02f);
            }

            yield return new WaitForSeconds(1);
            OnComplete?.Invoke();

        }

        public static IEnumerator TextCounter(TextMeshPro textMeshPro, int startValue, int endValue, float duration, System.Action OnComplete = null)
        {
            duration = Mathf.Clamp(duration, 1, 5);
            bool isIncrement = startValue < endValue;
            int divider = 1;
            string suffix = "";
            int decimalPlaces = 0;

            if (endValue >= 10000 && endValue < 1000000)
            {
                divider = 1000;
                suffix = "K";
                decimalPlaces = 1;
            }
            else if (endValue >= 1000000)
            {
                divider = 1000000;
                suffix = "M";
                decimalPlaces = 2;
            }

            float _startValue = (float)startValue / divider;
            float _endValue = (float)endValue / divider;
            int counterSteps = (int)(duration / 0.02f);
            float fraction = isIncrement ? (_endValue - _startValue) / counterSteps : (_startValue - _endValue) / counterSteps;

            for (int i = 0; i < counterSteps; i++)
            {
                if (isIncrement)
                    _startValue += fraction;
                else
                    _startValue -= fraction;

                textMeshPro.text = $"{_startValue.ToString("F" + decimalPlaces)}{suffix}";
                if (i == counterSteps - 1)
                    textMeshPro.text = $"{_endValue.ToString("F" + decimalPlaces)}{suffix}";

                yield return new WaitForSecondsRealtime(0.02f);
            }

            yield return new WaitForSeconds(1);
            OnComplete?.Invoke();

        }
    }
}