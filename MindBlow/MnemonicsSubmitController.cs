using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using static System.Guid;

namespace MindBlow
{
    [Route("api/[controller]")]
    [ApiController]
    public class MnemonicsSubmitController : ControllerBase
    {
        [HttpPost]
        public IActionResult PostMnemonics(HttpRequest request) {
            string filePath = "Mnemonics.json";

            using var reader = new StreamReader(request.Body);
            var body = reader.ReadToEndAsync().Result;

            var parsedForm = System.Web.HttpUtility.ParseQueryString(body);
            var textM = parsedForm["textM"];
            var textW = parsedForm["textW"];

            if (textM == "" | textW == "") {
                return Problem();
            }

            List<MnemonicsType> mnemonicsList;
            if (System.IO.File.Exists(filePath))
            {
                var jsonData = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    mnemonicsList = JsonSerializer.Deserialize<List<MnemonicsType>>(jsonData) ?? new List<MnemonicsType>();
                }
                else
                {
                    mnemonicsList = new List<MnemonicsType>();
                }
            }
            else
            {
                mnemonicsList = new List<MnemonicsType>();
            }

            var mnemonics = new MnemonicsType { 
                Id = Guid.NewGuid(),
                TextM = textM ?? string.Empty, 
                TextW = textW ?? string.Empty
            };

            mnemonicsList.Add(mnemonics);

            var updatedJson = JsonSerializer.Serialize(mnemonicsList, new JsonSerializerOptions { WriteIndented = true });

            System.IO.File.WriteAllText(filePath, updatedJson);

            return Ok("Mnemonics posted successfully");
        }
    }
}
