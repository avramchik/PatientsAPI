using PatientsAPI.Models.Dto;
using PatientsAPI.Models.Enums;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var patients = GetPatients();
        using (var client = new HttpClient())
        {
            foreach (var patient in patients)
            {
                using StringContent jsonContent = new(JsonSerializer.Serialize(patient), Encoding.UTF8, "application/json");

                var result = await client.PostAsync(new Uri("http://localhost:80/patient"), jsonContent).ConfigureAwait(false);
            }
        }
    }

    private static List<PatientDto> GetPatients()
    {
        var list = new List<PatientDto>();
        using (var client = new HttpClient())
        {
            var result = client.Send(new HttpRequestMessage
            {
                RequestUri = new Uri("https://randomuser.me/api/?results=100"),
                Method = HttpMethod.Get
            });

            var people = JsonNode.Parse(result.Content.ReadAsStream())["results"].AsArray();
            var rand = new Random();

            foreach (var peopleItem in people)
            {
                var date = peopleItem["dob"]["date"].GetValue<DateTime>();
                date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                var patient = new PatientDto
                {
                    Name = new Name
                    {
                        Family = peopleItem["name"]["last"].ToString(),
                        Use = "official",
                        Given = new string[] { peopleItem["name"]["first"].ToString() }
                    },
                    Gender = Enum.Parse<Gender>(peopleItem["gender"].GetValue<string>(), true),
                    BirthDate = date,
                    Active = Convert.ToBoolean(rand.Next(2))
                };
                list.Add(patient);
            }
            return list;
        }
    }
}