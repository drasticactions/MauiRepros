Using GenerateSelfSignedCert and SslStream fails on Android

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

TcpListener running on Android:

```
System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception.
 ---> Interop+AndroidCrypto+SslException: Exception of type 'Interop+AndroidCrypto+SslException' was thrown.
   --- End of inner exception stack trace ---
   at System.Net.Security.SslStream.<ForceAuthenticationAsync>d__150`1[[System.Net.Security.AsyncReadWriteAdapter, System.Net.Security, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
   at TestClientCore.TcpTestClient.ConnectAsync(String address, Int32 port) in /Users/drasticactions/Developer/Work/MauiRepros/TcpSslTest/TestClient/TestClientCore/TcpTestClient.cs:line 34
   at TestClientMaui.MainPage.Button4_OnClicked(Object sender, EventArgs e) in /Users/drasticactions/Developer/Work/MauiRepros/TcpSslTest/TestClient/TestClientMaui/MainPage.xaml.cs:line 31
   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__128_0(Object state)
   at Android.App.SyncContext.<>c__DisplayClass2_0.<Post>b__0() in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Android.App/SyncContext.cs:line 36
   at Java.Lang.Thread.RunnableImplementor.Run() in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Java.Lang/Thread.cs:line 36
   at Java.Lang.IRunnableInvoker.n_Run(IntPtr jnienv, IntPtr native__this) in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/obj/Release/net8.0/android-34/mcw/Java.Lang.IRunnable.cs:line 84
   at Android.Runtime.JNINativeWrapper.Wrap_JniMarshal_PP_V(_JniMarshal_PP_V callback, IntPtr jnienv, IntPtr klazz) in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Android.Runtime/JNINativeWrapper.g.cs:line 22
  
[System.err] Caused by: javax.net.ssl.SSLProtocolException: Read error: ssl=0x7461bd8f2e98: Failure in SSL library, usually a protocol error
[System.err] error:1000012e:SSL routines:OPENSSL_internal:KEY_USAGE_BIT_INCORRECT (external/boringssl/src/ssl/ssl_cert.cc:605 0x7460a5a53112:0x00000000)
[System.err] 	at com.android.org.conscrypt.NativeCrypto.ENGINE_SSL_read_direct(Native Method)
[System.err] 	at com.android.org.conscrypt.NativeSsl.readDirectByteBuffer(NativeSsl.java:569)
[System.err] 	at com.android.org.conscrypt.ConscryptEngine.readPlaintextDataDirect(ConscryptEngine.java:1095)
[System.err] 	at com.android.org.conscrypt.ConscryptEngine.readPlaintextDataHeap(ConscryptEngine.java:1115)
[System.err] 	at com.android.org.conscrypt.ConscryptEngine.readPlaintextData(ConscryptEngine.java:1087)
[System.err] 	at com.android.org.conscrypt.ConscryptEngine.unwrap(ConscryptEngine.java:876)

```
  