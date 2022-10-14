namespace MinimalAPIDemo;

public static class KeycloakConfigHelper
{
  public static IWebHostBuilder ConfigureKeycloak(this IWebHostBuilder webHostBuilder)
  {
    return webHostBuilder.ConfigureAppConfiguration((configBuilder) =>
    {

    }).ConfigureServices((builder, services) =>
    {

    });
  }

}
