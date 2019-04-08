using API_Usage.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API_Usage.Controllers
{
  public class AzureMLController : Controller
  {
    public async Task<IActionResult> Index()
    {
      AzureMLModel ViewModel = await InvokeRequestResponseService();

      return View(ViewModel);
    }

    static async Task<AzureMLModel> InvokeRequestResponseService()
    {
      AzureMLModel ViewModel = new AzureMLModel();

      using (var client = new HttpClient())
      {
        var scoreRequest = new
        {
          Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"sepallength", "sepalwidth",
                                                              "petallength", "petalwidth", "class"},
                                Values = new string[,] {  { "1", "2", "10", "3", "value" },
                                                            { "0", "0", "0", "0", "value" },  }
                            }
                        },
                    },
          GlobalParameters = new Dictionary<string, string>()
          {
          }
        };

        
        HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

        if (response.IsSuccessStatusCode)
        {
          string result = await response.Content.ReadAsStringAsync();
          RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(result.ToString());

          ViewModel.Message = "Result: " + result;
          ViewModel.JsonObject = rootObject.ToString();
        }
        else
        {
          
          ViewModel.Message = string.Format("The request failed with status code: {0}", response.StatusCode);
          ViewModel.Message += "\n" + response.Headers.ToString();

          string responseContent = await response.Content.ReadAsStringAsync();
          ViewModel.JsonObject = responseContent;
        }
      }

      return ViewModel;
    }
  }
}