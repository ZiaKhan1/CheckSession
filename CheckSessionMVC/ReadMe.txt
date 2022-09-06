1. There are two application. These applications uses "session management" specification(https://openid.net/specs/openid-connect-session-1_0.html)
to detect session changes & to respond the detected changes. 

2. One of the app uses Hybrid flow and the other uses Authorization Code flow

3. Each application checks every 30 seconds if session has changed or not.
Messages of "unchanged" or "changed" are logged to console accordingly. 

To test, do below.

4. Logout from one app. The second app will detect the change and logs user out as well. 
        Note that as mentioned earlier, the changes in session are being checked every 30 seconds 
        so it may take some time for second app to log the use out.

5. Logout and Login as another user from one app. The second app will also relogin & show the claims of the new user. The relogin process
involves logging out existing user from the app (but not from OP) and then logging again (i.e. sendiong authorize endpoint).
        Note that after logout from first app, you would need to login quickly as otherwise the second app my react to the logout state.
        The best is to initiate the logout & login in first app just after the second app logs the "unchanged" message to console 
        as it would then do the next check aftr 30 seconds giving you enough time to complete the logout\login from first app.

6. Logout and Login with the same user from one app. The second app will detect the changed state but also figure out that 
the user is same. It would just update the session_state value as per spec. You should be seeing messages of 
"Existing session_state:" and "Updated session_state:" in console log. 
        Note that after logout, you would need to login quickly as otherwise the second app my react to the logout state.
        The best is to initiate the logout & login from first app just after the second app logs the "unchanged" message to console 
        as it would then do the next check aftr 30 seconds giving you good time to do the logout\login from first app.



