using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    TimeOut,
    Error
}

public static class AuthenticationWrapper
{
    public static AuthState State { get; private set; } = AuthState.NotAuthenticated;
    public static event Action<string> OnMessageEvt;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (State == AuthState.Authenticated)
        {
            return State;
        }

        if (State == AuthState.Authenticating)
        {
            OnMessageEvt?.Invoke("인증 시도중");
            return await Authenticating();
        }

        await SignInAnonymouslyAsync(maxTries);

        return State;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        State = AuthState.Authenticating;

        int tried = 0;
        while (State == AuthState.Authenticating && tried < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    State = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException ex)
            {
                OnMessageEvt?.Invoke("서버점검중(실은 유니티 에러...)");
                State = AuthState.Error;
                break;
            }
            catch (RequestFailedException ex)
            {
                OnMessageEvt?.Invoke("인터넷 에러");
                State = AuthState.Error;
                break;
            }

            ++tried;
            await Task.Delay(1000); //1초
        }

        if (State != AuthState.Authenticated && tried >= maxTries)
        {
            OnMessageEvt?.Invoke("시간 초과");
            State = AuthState.TimeOut;
        }
    }

    private static async Task<AuthState> Authenticating()
    {
        while (State == AuthState.Authenticating)
        {
            await Task.Delay(200); //0.2초
        }
        return State;
    }
}
