// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;
// using Amazon;
// using Amazon.Polly;
// using Amazon.Polly.Model;
// using Amazon.Runtime;
// using UnityEngine;
// using UnityEngine.Networking;

// public class TextToSpeech : MonoBehaviour
// {
//     [SerializeField] private AudioSource audioSource;
//     // Start is called before the first frame update
//     public async void ReadText(string sceneText)
//     {
//         // var credentials = new BasicAWSCredentials("AKIAXJ4KPH7I5DOYZGYB", "Dwg9p5aHYFL8V7xH/j7CDWHgQ0loTQKIEhfpHTEc");
//         var credentials = new BasicAWSCredentials("AKIAXJ4KPH7I4K25GC6M", "Dwg9p5aHYFL8V7xH/9ETG66xLO0FqmIysf+2E0EdEFkTzZlFnhZS00+2E");
//         // var credentials = new BasicAWSCredentials("AKIAXJ4KPH7I5E5B3Y6H", "VR/TBsoxqiprnTDOpV9IJ0SIdNR3/MiZIWCMFbPW/9ETG66xLO0FqmIysf+2E0EdEFkTzZlFnhZS00+2E");
//         var client = new AmazonPollyClient(credentials, RegionEndpoint.APSoutheast1);

//         var request = new SynthesizeSpeechRequest(){
//             Text = sceneText,
//             Engine = Engine.Neural,
//             // Engine = Engine.Neural;
//             VoiceId = VoiceId.Joanna,
//             OutputFormat = OutputFormat.Mp3
//         };

//         var response = await client.SynthesizeSpeechAsync(request);
//         WriteIntoFile(response.AudioStream);
//         using(var www = UnityWebRequestMultimedia.GetAudioClip($"{Application.persistentDataPath}/audio.mp3", AudioType.MPEG)){
//             var op = www.SendWebRequest();
//             while(!op.isDone) await Task.Yield();
//             var clip = DownloadHandlerAudioClip.GetContent(www);
//             if (audioSource != null)
//             {
//                 audioSource.clip = clip;
//                 audioSource.Play();
//             }
//         }
//     }

//     private void WriteIntoFile(Stream stream){
//         using(var fileStream = new FileStream(path: $"{Application.persistentDataPath}/audio.mp3", FileMode.Create)){
//             byte[] buffer = new byte[8*1024];
//             int bytesRead = 0;
//             while((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0){
//                 fileStream.Write(buffer, 0, bytesRead);
//             }
//         }
//     }
// }
