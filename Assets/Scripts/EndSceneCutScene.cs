using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndSceneCutScene : MonoBehaviour
{
    public GameObject dog, human;
    public Transform dogPoint, humanPoint;
    public Transform[] desiredCameraTransform;
    private GameObject sceneChangerGO;
    private bool humanFinished = false, dogFinished = false, fading = false;

    private Delegate tempDelegate;
    private List<Delegate> delegateList;
    private delegate void Delegate();
    float humanMaxDeltaDistanceFactor = 6f, dogMaxDeltaDistanceFactor = 7f;
    int counter = 0;

    private float transitionSpeed1, transitionSpeed2;
    private bool cameraMove = false;

    // Start is called before the first frame update
    void Start()
    {
        sceneChangerGO = GameObject.Find("SceneChanger");
        delegateList = new List<Delegate>();
        transitionSpeed1 = 0.35f;
        transitionSpeed2 = 0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (delegateList.Count != 0)
            delegateList[0].Method.Invoke(this, null);

        if (cameraMove)
            MoveCamera();

        if (Input.GetKeyDown(KeyCode.Return) && delegateList.Count == 0)
        {
            cameraMove = true;
            tempDelegate = new Delegate(Deactivate);
            delegateList.Add(tempDelegate);
            tempDelegate = new Delegate(AllRunToPoint);
            delegateList.Add(tempDelegate);
        }
    }

    private void Deactivate()
    {  
        //Deactivates the humans components
        human.gameObject.GetComponent<CharacterController>().enabled = false;
        human.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        human.gameObject.GetComponent<HumanMovement>().enabled = false;
        human.gameObject.GetComponent<HumanAI>().enabled = false;
        humanFinished = false;

        //Deactivates the dogs components
        dog.gameObject.GetComponent<CharacterController>().enabled = false;
        dog.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        dog.gameObject.GetComponent<DogMovement>().enabled = false;
        dog.gameObject.GetComponent<DogAI>().enabled = false;
        dogFinished = false;

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

        if (Vector3.Distance(Camera.main.transform.position, desiredCameraTransform[counter].position) < 3f)
        {
            if ((counter+1) < desiredCameraTransform.Length)
            {
                counter++;
            }
            if (counter == 1 && Vector3.Distance(Camera.main.transform.position, desiredCameraTransform[counter].position) < 3f && !fading)
            {
                fading = true;
                sceneChangerGO.GetComponent<SceneChanger>().FadeToScene(0);
            }

            transitionSpeed1 = transitionSpeed2;
        }

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredCameraTransform[counter].position, transitionSpeed1 * Time.deltaTime);

        Vector3 currentAngle = new Vector3(
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.x, desiredCameraTransform[counter].rotation.eulerAngles.x, transitionSpeed1 * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, desiredCameraTransform[counter].rotation.eulerAngles.y, transitionSpeed1 * Time.deltaTime),
            Mathf.LerpAngle(Camera.main.transform.eulerAngles.z, desiredCameraTransform[counter].rotation.eulerAngles.z, transitionSpeed1 * Time.deltaTime));

        Camera.main.transform.eulerAngles = currentAngle;
    }

    private void AllRunToPoint()
    {
        float humanDistance, dogDistance;
        humanDistance = Vector3.Distance(human.transform.position, humanPoint.position);
        dogDistance = Vector3.Distance(dog.transform.position, dogPoint.position);
        //The Humans movement to a point
        if (humanDistance < 7)
        {
            cameraMove = true;
        }

        if (humanDistance > .3f)
        {
            human.gameObject.GetComponent<Animator>().SetBool("Running", true);
            if (humanMaxDeltaDistanceFactor > 5)
            {
                humanMaxDeltaDistanceFactor -= 0.3f;
            }
            human.GetComponent<Animator>().speed = humanMaxDeltaDistanceFactor / 5;
            human.transform.position = Vector3.MoveTowards(human.transform.position, humanPoint.position, humanMaxDeltaDistanceFactor * Time.deltaTime);
            human.transform.LookAt(humanPoint.transform);
        }
        else
        {
            human.gameObject.GetComponent<Animator>().SetBool("Running", false);
            humanFinished = true;
        }

        //The Dogs movement to a point
        if (dogDistance > .3f)
        {
            dog.gameObject.GetComponent<Animator>().SetBool("Running", true);
            if (dogMaxDeltaDistanceFactor > 5)
            {
                dogMaxDeltaDistanceFactor -= 0.3f;
            }
            dog.GetComponent<Animator>().speed = humanMaxDeltaDistanceFactor / 5;
            dog.transform.position = Vector3.MoveTowards(dog.transform.position, dogPoint.position, dogMaxDeltaDistanceFactor * Time.deltaTime);
            dog.transform.LookAt(dogPoint.transform);
        }
        else
        {
            dog.gameObject.GetComponent<Animator>().SetBool("Running", false);
            dogFinished = true;
        }

        if (humanFinished && dogFinished)
        {
            if (delegateList.Count > 0)
            {
                delegateList.RemoveAt(0);
            }
        }
    }

    #region#unused#
    //private void HumanRunToPoint()
    //{
    //    if (Vector3.Distance(human.transform.position, humanPoint.position) > .3f)
    //    {
    //        if (Vector3.Distance(human.transform.position, humanPoint.position) <= .3f)
    //        {
    //            human.gameObject.GetComponent<Animator>().SetBool("Running", false);
    //        }
    //        else
    //            human.gameObject.GetComponent<Animator>().SetBool("Running", true);


    //        if (maxDeltaDistanceFactor < 15)
    //        {
    //            maxDeltaDistanceFactor += 0.3f;
    //        }
    //        human.GetComponent<Animator>().speed = maxDeltaDistanceFactor / 10;
    //        human.transform.position = Vector3.MoveTowards(human.transform.position, humanPoint.position, maxDeltaDistanceFactor * Time.deltaTime);
    //        human.transform.LookAt(humanPoint.transform);
    //    }
    //    else
    //    {
    //        if (delegateList.Count > 0)
    //        {
    //            delegateList.RemoveAt(0);
    //        }
    //    }
    //}

    //private void DogRunToPoint()
    //{
    //    if (Vector3.Distance(dog.transform.position, dogPoint.position) > .3f)
    //    {
    //        if (Vector3.Distance(dog.transform.position, dogPoint.position) <= .3f)
    //        {
    //            dog.gameObject.GetComponent<Animator>().SetBool("Running", false);
    //        }
    //        else
    //            dog.gameObject.GetComponent<Animator>().SetBool("Running", true);


    //        if (maxDeltaDistanceFactor < 15)
    //        {
    //            maxDeltaDistanceFactor += 0.3f;
    //        }
    //        dog.GetComponent<Animator>().speed = maxDeltaDistanceFactor / 10;
    //        dog.transform.position = Vector3.MoveTowards(dog.transform.position, dogPoint.position, maxDeltaDistanceFactor * Time.deltaTime);
    //        dog.transform.LookAt(dogPoint.transform);
    //    }
    //    else
    //    {
    //        if (delegateList.Count > 0)
    //        {
    //            delegateList.RemoveAt(0);
    //        }
    //    }
    //}

    //private void TurnTowardsPoint()
    //{
    //    Vector3 dogVector = dogPoint.transform.position - dog.transform.position;
    //    dogVector.Normalize();
    //    Vector3 desiredDogRot = new Vector3(dogVector.x, 0, dogVector.z);

    //    if (Vector3.Angle(dog.transform.forward, desiredDogRot) > 2)
    //    {
    //        Quaternion dogLookRot = Quaternion.LookRotation(desiredDogRot);
    //        dog.transform.rotation = Quaternion.Lerp(dog.transform.rotation, dogLookRot, 2f * Time.deltaTime);
    //    }
    //    //else
    //    //{
    //    //    delegateList.RemoveAt(0);
    //    //}
    //}
    #endregion
}
