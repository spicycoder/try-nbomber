using System;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace WeatherAPI.LoadTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var forecastScenario = ForecastTest();
            var historyScenario = HistoryTest();

            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] { "forecast", "history" });
            var pingPlugin = new PingPlugin(pingPluginConfig);

            NBomberRunner
                .RegisterScenarios(forecastScenario, historyScenario)
                .WithWorkerPlugins(pingPlugin)
                .Run();
        }

        public static Scenario ForecastTest()
        {
            var step = Step.Create(
                "forecast",
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", "https://localhost:5001/WeatherForecast/forecast")
                        .WithHeader("Accept", "application/json");

                    return Http.Send(request, context);
                });

            var scenario = ScenarioBuilder
                .CreateScenario("get_forecast", step)
                .WithLoadSimulations(LoadSimulation.NewInjectPerSec(100, TimeSpan.FromSeconds(60)));

            return scenario;
        }

        public static Scenario HistoryTest()
        {
            var step = Step.Create(
                "history",
                HttpClientFactory.Create(),
                context =>
                {
                    var request = Http.CreateRequest("GET", "https://localhost:5001/WeatherForecast/history")
                        .WithHeader("Accept", "application/json");

                    return Http.Send(request, context);
                });

            var scenario = ScenarioBuilder
                .CreateScenario("get_history", step)
                .WithLoadSimulations(LoadSimulation.NewInjectPerSec(100, TimeSpan.FromSeconds(60)));

            return scenario;
        }
    }
}
