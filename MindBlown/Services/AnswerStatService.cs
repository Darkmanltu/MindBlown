using System.Net.Http.Json;
using Microsoft.JSInterop;
using MindBlown.Types;
using System.IdentityModel.Tokens.Jwt; // For JWT handling
using System.Security.Claims;  
using System;
using System.Text;
using System.Text.Json;
using MindBlown.Interfaces;

public class AnswerStatService {

     private readonly HttpClient _httpClient;
        
        
     public AnswerStatService(HttpClient httpClient)
    {
       
        _httpClient = httpClient;
    }

    public async Task<List<AnswerSessionType>> GetList(string username){
        var list = await _httpClient.GetFromJsonAsync<List<AnswerSessionType>>($"api/answersession/list?user={Uri.EscapeDataString(username)}") ?? new List<AnswerSessionType>();
        return list;
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
            
           //  Console.WriteLine("Trying to create session: " + answerSession.AnswerSessionId + " and: " + answerSession.CorrectCount );

        
        if (answerMnemonics == null || !answerMnemonics.Any())
        {
            Console.WriteLine("empty list");
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
            
             //Console.WriteLine("AnswerSession: " + JsonSerializer.Serialize(answerSession));
            

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
                          //  Console.WriteLine("Error answredmnem: " + resp.ReasonPhrase);
                            // Console.WriteLine("AnsweredMnemonics: " + JsonSerializer.Serialize(ans));
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