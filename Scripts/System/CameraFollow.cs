using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float cameraSpeed = 5.0f;
    public GameObject player;
    bool isOver = false;

    private void FixedUpdate()
    {
        if (isOver)
            return;

        Vector3 dir = player.transform.position - transform.position; //카메라와 플레이어의 거리 계산하여 방향 구하기
        Vector3 moveVec = new Vector3(dir.x * cameraSpeed * Time.deltaTime, dir.y * cameraSpeed * Time.deltaTime, 0.0f); //해당 방향으로 이동하는 벡터 값
        transform.Translate(moveVec);
    }

    public void PlayerDeathCameraEffect(Transform playerTransform)
    {
        isOver = true;
        StartCoroutine(CameraZoomAndFocus(playerTransform.position, 2.5f, 1f));
    }

    public void HeartQueenDeathCameraEffect(Transform bossTransform)
    {
        isOver = true;
        StartCoroutine(CameraZoomAndFocus(bossTransform.position, 2.5f, 1f));
    }

    private IEnumerator CameraZoomAndFocus(Vector3 targetPosition, float zoomTarget, float duration)
    {
        Camera cam = Camera.main;
        float elapsed = 0f;

        Vector3 startPos = cam.transform.position;
        Vector3 targetPos = new Vector3(targetPosition.x, targetPosition.y, startPos.z);

        float startSize = cam.orthographicSize;

        //애니메이션 중 Time.timeScale = 0 이어도 작동하도록 unscaledDeltaTime 사용
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            //이동 & 줌 보간
            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, zoomTarget, t);

            yield return null;
        }

        //보정
        cam.transform.position = targetPos;
        cam.orthographicSize = zoomTarget;
    }
}
