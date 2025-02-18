using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using CharacterInfo;
public class MainStoryFirst : StoryManager
{
    public GameObject personOld;
    async public void Start()
    {
        //await LoadStory();
        await MainQuestStart();
       
    }
    async public Task LoadStory()
    {
        int intchange = (int)Mathf.FloorToInt(CharacterManager.Instance.characterPersonalinfo.storyNum);
        if (intchange == 0)
        {

        }
        return;
    }
    async public Task MainQuestStart()
    {
        await StartStory("MainStory/MS_1", "MainFirst");
        StartCoroutine(MoveAndLookAtTarget(new Vector3(7, 0, 1)));
        return;
    }
    private IEnumerator MoveAndLookAtTarget(Vector3 targetPosition)
    {
        Debug.Log(targetPosition);
        personOld.GetComponent<Animator>().SetBool("IsWalk", true);

        while (Vector3.Distance(personOld.transform.position, targetPosition) > 0.1f)
        {
            // 목표를 향해 천천히 회전
            Vector3 direction = (targetPosition - personOld.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            personOld.transform.rotation = Quaternion.Slerp(personOld.transform.rotation, lookRotation, Time.deltaTime * 2.0f);

            // 목표를 향해 이동
            personOld.transform.position = Vector3.MoveTowards(personOld.transform.position, targetPosition, 1.0f * Time.deltaTime);

            yield return null;  // 다음 프레임까지 대기
        }
        personOld.GetComponent<Animator>().SetBool("IsWalk", false);

        Quaternion targetRotation = Quaternion.Euler(personOld.transform.eulerAngles.x, personOld.transform.eulerAngles.y - 90f, personOld.transform.eulerAngles.z);

        while (Quaternion.Angle(personOld.transform.rotation, targetRotation) > 0.1f)
        {
            personOld.transform.rotation = Quaternion.Slerp(personOld.transform.rotation, targetRotation, Time.deltaTime * 2.0f);

            yield return null;  // 다음 프레임까지 대기
        }
    }

}
