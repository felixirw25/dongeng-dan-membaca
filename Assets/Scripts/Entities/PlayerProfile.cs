using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    private int hintCount;
    private int starCount;
    private int starSpentCount;
    public List<List<int>> starPerLevelCerita;
    public List<string> isFinishedList;
    public List<DocumentSnapshot> pakaianOwned;
    public PlayerProfile thePlayer;
    public int HintCount { get => hintCount; set => hintCount = value; }
    public int StarCount { get => starCount; set => starCount = value; }
    public int StarSpentCount { get => starSpentCount; set => starSpentCount = value; }

    public PlayerProfile(int listCeritaCount){
        starPerLevelCerita = new List<List<int>>();
        isFinishedList = new List<string>();

        for(int i = 0; i < listCeritaCount; i++){
            starPerLevelCerita.Add(new List<int>());
            
            isFinishedList.Add("false");
        }
    }

    public int CountStarCerita(int idCerita){
        int tempStar = 0;
        foreach (var starLevel in starPerLevelCerita[idCerita]){
            tempStar += starLevel;
        }
        return tempStar;
    }

    public int CountStarAll(){
        int tempStar = 0;
        for(int i = 0; i < starPerLevelCerita.Count; i++){
            foreach (var starLevel in starPerLevelCerita[i]){
                tempStar += starLevel;
            }
        }

        StarCount = tempStar;
        return StarCount;
    }

    public void UpdateStar(int cerita, int level, int starResult){
        starPerLevelCerita[cerita][level] = starResult;
        CountStarAll();
    }

    public async Task<Dictionary<string, object>> ExportDataAsync(int index, int id_cerita, string id_dokumen, int variasiAdegan){
        Dictionary<string, object> exported = new Dictionary<string, object>();
        List<object> starPerLevelString = new List<object>();
        if (starPerLevelCerita[index-1].Count==0)
        {
            FirebaseFirestore firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
            await firestore.Collection("stories").Document(id_dokumen).Collection("scenes").GetSnapshotAsync().ContinueWithOnMainThread(task=>{
                for (int i = 0; i < Mathf.FloorToInt(task.Result.Count/variasiAdegan); i++)
                {
                    starPerLevelString.Add("0");
                }
                exported.Add("id_cerita", id_cerita);
                exported.Add("starPerLevel", starPerLevelString);
                exported.Add("isFinished", isFinishedList[index-1]);

            });
            return exported;
            // FirebaseFirestore firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
            // await firestore.Collection("stories").Document(id_dokumen).Collection("scenes").GetSnapshotAsync().ContinueWithOnMainThread(task=>{
            //     for (int i = 0; i < Mathf.FloorToInt(task.Result.Count/3); i++)
            //     {
            //         starPerLevelString.Add("0");
            //     }
            //     exported.Add("id_cerita", id_cerita);
            //     exported.Add("starPerLevel", starPerLevelString);
            //     exported.Add("isFinished", isFinishedList[index-1]);

            // });
            // return exported;
        }
        else{
            foreach (int starPerLevel in starPerLevelCerita[index-1])
            {
                starPerLevelString.Add(starPerLevel.ToString());
            }
            exported.Add("id_cerita", id_cerita);
            exported.Add("starPerLevel", starPerLevelString);
            exported.Add("isFinished", isFinishedList[index-1]);
            
            return exported;
        }
    }
}