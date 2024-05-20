Android on Windows, Error thrown on Android

```
[DOTNET] System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception.
[DOTNET]  ---> Interop+AndroidCrypto+SslException: Exception of type 'Interop+AndroidCrypto+SslException' was thrown.
[DOTNET]    --- End of inner exception stack trace ---
[DOTNET]    at System.Net.Security.SslStream.<ForceAuthenticationAsync>d__150`1[[System.Net.Security.AsyncReadWriteAdapter, System.Net.Security, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
[DOTNET]    at TestClientCore.TcpTestClient.ConnectAsync(String address, Int32 port) in C:\Users\timill\source\repos\MauiRepros\TcpSslTest\TestClient\TestClientCore\TcpTestClient.cs:line 28
[DOTNET]    at TestClientMaui.MainPage.Button_OnClicked(Object sender, EventArgs e) in C:\Users\timill\source\repos\MauiRepros\TcpSslTest\TestClient\TestClientMaui\MainPage.xaml.cs:line 36
[DOTNET] Interop+AndroidCrypto+SslException: Exception of type 'Interop+AndroidCrypto+SslException' was thrown.
[EGL_emulation] app_time_stats: avg=18.19ms min=9.39ms max=123.67ms count=35
[ProfileInstaller] Installing profile for com.companyname.testclientmaui
```

Android on MacOS, Error thrown on Server

```
System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception.
 ---> Interop+AppleCrypto+SslException: bad protocol version
   --- End of inner exception stack trace ---
   at System.Net.Security.SslStream.ForceAuthenticationAsync[TIOAdapter](Boolean receiveFirst, Byte[] reAuthenticationData, CancellationToken cancellationToken)
   at System.Net.Security.SslStream.AuthenticateAsServer(SslServerAuthenticationOptions sslServerAuthenticationOptions)
   at Program.<Main>$(String[] args) in /Users/drasticactions/Developer/Work/MauiRepros/TcpSslTest/TestServer/Program.cs:line 28
   at Program.<Main>(String[] args)
```

Server running on MacOS with iOS, Mac works. Server running on Windows running WinUI Works (Provided cert is created locally on disk)