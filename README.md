# BelTwit-REST-API



## AuthController
| Method    | URL                                           | Body       | Description                                                 |
| :-------: | :-------------------------------------------- | :--------- | :-----------------------------------------------------------|
| GET    | api/auth     || Get all the users.
| POST   | api/auth/create        |[User model](#user-json-model)| Create a single user. Login(unique, length: [2;20]) and password(length: [5;100]) are required.
| PUT    | api/auth/update        |[2 users tuple](#2-users-json-tuple)| Update the user by changing property values. Item1 represents OldUser(old login and password are required), and Item2 representes NewUser(all changes are here).
| DELETE | api/auth/delete        |[User model](#user-json-model)| Deletes the user(password and login are required).
| POST   | api/auth/authentificate|[User model](#user-json-model)| Authentificate the user by creating JWT(30 min) and RefreshToken(60 days).
| GET    | api/auth/authorize     |`"YourJWTValue"`| Authorize the user by JWT value as a string.
| POST   | api/auth/update-tokens|[AccessRefreshToken model](#accessrefreshtoken-json-model)| Refresh your JWT[or AccessToken] (for 30 minutes) and your RefreshToken (for 60 days). 
| POST   | api/auth/subscribe     |[UserSubscribe model](#usersubscribe/unsubscribe-json-model)| Subscribes on the other user by your own JWT value and login of the other user.
| DELETE | api/auth/unsubscribe   |[UserUnsubscribe model](#usersubscribe/unsubscribe-json-model)| Unsubscribes from the other user by your own JWT value and login of the other user.
| GET | api/auth/get-subscribers  |`"YourJWTValue"`| Get your subscribers(who subscribed on you) by JWT value.
| GET | api/auth/get-subscriptions|`"YourJWTValue"`| Get your subscriptions(on whom you subscribed) by JWT value.



## Controllers bodies:

#### User JSON model:
```json
{
  "Login": "yourLogin",
  "Password": "yourPassword"
}
```
#### 2-users JSON tuple:
```json
{
	"Item1": {
		"Login": "oldLogin",
		"Password": "oldPassword"
	},
	"Item2": {
		"Login": "newLogin",
		"Password": "newPassword"
	}
}
```
#### AccessRefreshToken JSON model:
```json
{
  "AccessToken": "yourJWTValue",
  "RefreshToken": "yourRefreshTokenValue",
}
```
#### UserSubscribe/Unsubscribe JSON model:
```json
{
	"JWT": "yourJWTValue",
	"OtherUserLogin": "UserLogin"
}
```