using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TitleSceneCutScene : MonoBehaviour
{

    public GameObject dog;
    public Transform[] dogPoints;
    public Transform desiredCameraTransform;
    private int counter = 0;
    GameObject sceneChangerGO;

    private Delegate tempDelegate;
    private List<Delegate> delegateList;
    private delegate void Delegate();
    float maxDeltaDistanceFactor = 5f;

    private float transitionSpeed;
    private bool over, cameraMove = false;

    // Start is called before the first frame update
    void Start()
    {
        sceneChangerGO = GameObject.Find("SceneChanger"); 
        delegateList = new List<Delegate>();
        transitionSpeed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(dog.transform.position, dogPoints[0].position) < 2f)
        {
            if (sceneChangerGO.TryGetComponent(out SceneChanger sceneChanger))
            {
                sceneChanger.FadeToNextScene();
            }

        }
            if (delegateList.Count != 0)
            delegateList[0].Method.Invoke(this, null);

        if (cameraMove)
            MoveCamera();

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("StartButton")) && delegateList.Count == 0)
        {
            tempDelegate = new Delegate(Deactivate);
            delegateList.Add(tempDelegate);
            tempDelegate = new Delegate(DogRunToPoint);
            delegateList.Add(tempDelegate);
            tempDelegate = new Delegate(DogRunToPoint);
            delegateList.Add(tempDelegate);
            cameraMove = true;
            dog.TryGetComponent(out MoreAudioClips clips);
            clips.PlayClipDelayed(1, 0.3f);
        }
    }

    private void Deactivate()
    {
        //Deactivates the dogs components
        dog.gameObject.GetComponent<CharacterController>().enabled = false;
        dog.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        dog.gameObject.GetComponent<DogMovement>().enabled = false;
        dog.gameObject.GetComponent<DogAI>().enabled = false;
        if (delegateList.Count > 0)
        {
            delegateList.RemoveAt(0);
        }
    }

    private void MoveCamera()
    {
        if (Camera.main.TryGetComponent(out ThirdPersonView tPV))
        {
            tPV.enabled = false;
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredCameraTransform.position, transitionSpeed * Time.deltaTime);

        Vector3 currentAngle = new Vector3(
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, desiredCameraTransform.rotation.eulerAngles.x, transitionSpeed * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, desiredCameraTransform.rotation.eulerAngles.y, transitionSpeed * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, desiredCameraTransform.rotation.eulerAngles.z, transitionSpeed * Time.deltaTime));

        Camera.main.transform.eulerAngles = currentAngle;
    }

    private void TurnTowardsPoint()
    {
        Vector3 dogVector = dogPoints[counter].transform.position - dog.transform.position;
        dogVector.Normalize();
        Vector3 desiredDogRot = new Vector3(dogVector.x, 0, dogVector.z);
        //dog.gameObject.GetComponent<Animator>().SetBool("Running", false);

        if (Vector3.Angle(dog.transform.forward, desiredDogRot) > 2)
        {
            Quaternion dogLookRot = Quaternion.LookRotation(desiredDogRot);
            dog.transform.rotation = Quaternion.Lerp(dog.transform.rotation, dogLookRot, 2f * Time.deltaTime);
        }
        //else
        //{
        //    delegateList.RemoveAt(0);
        //}
    }

    private void DogRunToPoint()
    {
        if (Vector3.Distance(dog.transform.position, dogPoints[1].position) > .3f)
        {
            if (Vector3.Distance(dog.transform.position, dogPoints[1].position) <= .3f)
            {
                dog.gameObject.GetComponent<Animator>().SetBool("Running", false);
            }
            else
                dog.gameObject.GetComponent<Animator>().SetBool("Running", true);


            if (maxDeltaDistanceFactor < 15)
            {
                maxDeltaDistanceFactor += 0.3f;
            }
            dog.GetComponent<Animator>().speed = maxDeltaDistanceFactor / 10;
            dog.transform.position = Vector3.MoveTowards(dog.transform.position, dogPoints[1].position, maxDeltaDistanceFactor * Time.deltaTime);
            dog.transform.LookAt(dogPoints[1].transform);
        }
        else
        {
            if (delegateList.Count > 0)
            {
                delegateList.RemoveAt(0);
            }
        }
    }
}
