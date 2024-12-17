using System.Net.Http.Json;
using Microsoft.JSInterop;
using MindBlown.Types;
using System.IdentityModel.Tokens.Jwt; // For JWT handling
using System.Security.Claims;  
using System;
using System.Text;
using System.Text.Json;
using MindBlown.Interfaces;
using System.Net;

public class AnswerStatService {

     private readonly HttpClient _httpClient;
        
        
     public AnswerStatService(HttpClient httpClient)
    {
       
        _httpClient = httpClient;
    }

    public async Task<List<AnswerSessionType>> GetList(string username){
        var response = await _httpClient.GetAsync($"api/answersession/list?user={Uri.EscapeDataString(username)}");

         if (response.IsSuccessStatusCode)
         {
             // Parse the JSON only if the request was successful (status code 200-299)
             var list = await response.Content.ReadFromJsonAsync<List<AnswerSessionType>>() ?? new List<AnswerSessionType>();
             return list;
         }
         else if (response.StatusCode == HttpStatusCode.NotFound)
         {
             // Handle 404 Not Found case here
             return new List<AnswerSessionType>();
         }
    }

    public async Task<bool> CreateAnswerSessionAsync(AnswerSessionType answerSession){

        try {
            var response = await _httpClient.PostAsJsonAsync("api/answersession/add", answerSession);
            await Task.Delay(100);
            return true;
        }
        catch (Exception e1)
            {
                Console.WriteLine("error: " + e1);
                return false;
            }

    }
    public async Task <bool> AddAnsweredMnemonicAsync(AnsweredMnemonicType answeredMnemonic){
        if (answeredMnemonic != null){
            await _httpClient.PostAsJsonAsync("api/answersession/addAnsweredMnemonic", answeredMnemonic);
        }   
        return true;
    }
     public async Task<bool> AddAnswerSessionAsync(AnswerSessionType answerSession, ICollection<AnsweredMnemonicType> answerMnemonics)
    {
            
           
        
        if (answerMnemonics == null || !answerMnemonics.Any())
        {
            
            return false;
        }

        try {
            var response = await _httpClient.PostAsJsonAsync("api/answersession/add", answerSession);   
        }
        catch (Exception e1)
            {
                Console.WriteLine("error: " + e1);
                return false;
            }

        try
        {   
            // Sending POST request to the AnswerSessionController API endpoint
                try
                {
                    foreach (var m in answerMnemonics)
                    {
                        var ans = new AnsweredMnemonicType
                        {
                            AnsweredMnemonicId = Guid.NewGuid(),
                            AnswerSessionId = m.AnswerSessionId,
                            IsCorrect = m.IsCorrect,
                            MnemonicId = m.MnemonicId,
                            AnswerSession = answerSession
                           
                        };
                        var resp = await _httpClient.PostAsJsonAsync("api/answersession/addAnsweredMnemonic", ans);
                        if (!resp.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Error bad answered mnemonic: " + resp.ReasonPhrase);
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Error adding answered mnemonics: " + ex1.Message);
                    return false;
                }

            
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding answer session: " + ex.Message);
            return false;
        }
    }
}
