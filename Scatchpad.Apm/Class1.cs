using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5000");

var activitySource = new ActivitySource("temp");
var listener = new HttpClientGlobalListener();
using var subscription = DiagnosticListener.AllListeners.Subscribe(listener);


using (var activity = activitySource.StartActivity("first request", ActivityKind.Client))
{
    //Request with Request-Id header
    var request = new HttpRequestMessage(HttpMethod.Get, "/"); 
    request.Headers.Add("Request-Id", "Hello");
    var response = httpClient.SendAsync(request).Result;
    response.EnsureSuccessStatusCode();
    var res = response.Content.ReadAsStringAsync().Result;
    Console.WriteLine(res);
}


using (var activity2 = activitySource.StartActivity("second request", ActivityKind.Client))
{
    //Request without Request-Id header
    var request = new HttpRequestMessage(HttpMethod.Get, "/"); 
    var response = httpClient.SendAsync(request).Result;
    response.EnsureSuccessStatusCode();
    var res = response.Content.ReadAsStringAsync().Result;
    Console.WriteLine(res);
}



public class HttpClientGlobalListener : IObserver<DiagnosticListener>
{
    private readonly HttpClientInterceptor _interceptor = new HttpClientInterceptor();

    public void OnCompleted() { }

    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener listener)
    {
        Console.WriteLine(listener.Name);
        listener.Subscribe(_interceptor);
    }
}

public class HttpClientInterceptor : IObserver<KeyValuePair<string, object>>
{
    public void OnCompleted() { }

    public void OnError(Exception error) { }

    public void OnNext(KeyValuePair<string, object> value)
    {
        Console.WriteLine(value.Key);
        Console.WriteLine(value.Value);
    }
}