# BelTwit-REST-API
## Features of the project


## UserController
| Method    | URL | Body | Description | Status codes |
| :-------: | :-- | :--- | :---------- | :----------- |
|__OPTIONS__| api/user/get-subscribers||||`200`
| HEAD   | api/user/get-subscribers||||`200`, `400`, `404`
| GET    | api/user/get-subscribers|`"YourJWTValue"`| Get your subscribers(who subscribed on you) by JWT value.|`200`, `400`, `404`
|__OPTIONS__| api/user/subscribe      ||||`200`
| HEAD   | api/user/subscribe 	   ||||`200`, `400`, `404`
| GET 	 | api/user/subscribe|`"YourJWTValue"`| Get your subscriptions(on whom you subscribed) by JWT value.|`200`, `400`, `404`
| POST   | api/user/subscribe      |[UserSubscribe model](#usersubscribe/unsubscribe-json-model)| Subscribes on the other user by your own JWT value and login of the other user.|`200`, `400`, `404`
| DELETE | api/user/subscribe      |[UserUnsubscribe model](#usersubscribe/unsubscribe-json-model)| Unsubscribes from the other user by your own JWT value and login of the other user.|`200`, `400`, `404`
|__OPTIONS__| api/user 			   ||||`200`
| HEAD   | api/user 			   ||||`200`
| GET    | api/user     		   || Get all the users.|`200`
| POST   | api/user        	 	   |[User model](#user-json-model)| Create a single user. Login(unique, length: [2;20]) and password(length: [5;100]) are required.|`200`, `403`, `404`
| PUT    | api/user       	 	   |[2 users tuple](#2-users-json-tuple)| Update the user by changing property values. Item1 represents OldUser(old login and password are required), and Item2 representes NewUser(all changes are here).|`200`, `403`, `404`
| DELETE | api/user        	 	   |[User model](#user-json-model)	  | Deletes the user(password and login are required).|`200`, `403`, `404`
|__OPTIONS__| api/admin-delete 	   ||||`200`
| DELETE | api/user/admin-delete   |[JwtWithUserId model](#jwtwithid-json-model)| Deletes the user by Id. Only for admnistrators!|`200`, `400`, `403`, `404`




## AuthController
| Method    | URL | Body | Description | Status codes |
| :-------: | :-- | :--- | :---------- | :----------- |
|__OPTIONS__| api/auth 			 ||||`200`
| HEAD   | api/auth 			 ||||`200`, `400`
| GET    | api/auth     	 	 |`"YourJWTValue"`| Authorize the user by JWT value as a string.|`200`, `400`
| POST   | api/auth	 			 |[User model](#user-json-model)| Authentificate the user by creating JWT(30 min) and RefreshToken(60 days).|`200`, `403`, `404`
|__OPTIONS__| api/auth/update-tokens||||`200`
| POST   | api/auth/update-tokens|[AccessRefreshToken model](#accessrefreshtoken-json-model)  | Refresh your JWT[or AccessToken] (for 30 minutes) and your RefreshToken (for 60 days).|`200`, `400`



## TwitterController
| Method    | URL | Body | Description | Status codes |
| :-------: | :-- | :--- | :---------- | :----------- |
|__OPTIONS__| api/twitter/getById/{id} 	 ||||`200`
| HEAD   | api/twitter/getById/{id} 	 ||||`200`, `400`, `404`
| GET    | api/twitter/getById/{id}      || Get tweet by "id".|`200`, `400`, `404`
| OPTIONS| api/twitter/getByLogin/{login}||||`200`
| HEAD   | api/twitter/getByLogin/{login}||||`200`, `404`
| GET    | api/twitter/getByLogin/{login}|| Get all tweets of user with "login".|`200`, `404`
|__OPTIONS__| api/twitter 					 ||||`200`
| HEAD   | api/twitter 					 ||||`200`, `400`, `404`
| GET    | api/twitter  				 |`"YourJWTValue"`| Get tweets of your subscriptions(users on whom you've subscribed). JWT required.|`200`, `400`, `404`
| POST   | api/twitter                   |[JwtWithTweet model](#jwtwithtweet-json-model)  | Creates a user tweet. JWT and Tweet (Content at least) required.|`200`, `400`, `404`
| DELETE | api/twitter   				 |[JwtWithTweetId model](#jwtwithid-json-model)   | Deletes a user tweet. JWT and TweetId required. Admin can delte any tweet.|`200`, `400`, `404`
|__OPTIONS__| api/twitter/comment-tweet     ||||`200`
| POST   | api/twitter/comment-tweet	 |[CommentAdding model](#commentadding-json-model)| Comments selected tweet(by TweetId). JWT required.|`200`, `400`, `404`
| DELETE | api/twitter/comment-tweet	 |[JwtWithCommentId model](#jwtwithid-json-model) | Delete comment by Id. JWT and TweetId required. Admin can delte any comment.|`200`, `400`, `404`
|__OPTIONS__| api/twitter/retweet   		 ||||`200`
| POST   | api/twitter/retweet	 		 |[JwtWithTweetId model](#jwtwithid-json-model)	  | Retweet selected tweet to your user. JWT required.|`200`, `400`, `404`
|__OPTIONS__| api/twitter/rate-tweet   	 ||||`200`
| PUT    | api/twitter/rate-tweet		 |[RatingAdding model](#ratingadding-json-model)  | Rates selected tweet(by TweetId). There are 3 possible rates `"Dislike"`,`"None"`,`"Like"`. JWT required.|`200`, `400`, `404`







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
	"WithJWTObject": "UserLogin"
}
```
#### JwtWithTweet JSON model:
```json
{
  	"JWT": "yourJWTValue",
  	"WithJWTObject": {
	  	"Content": "It is my tweet"
  }
}
```
#### JwtWithId JSON model:
```json
{
  	"JWT": "yourJWTValue",
  	"WithJWTObject": "yourTweetOrCommentId"
}
```

#### CommentAdding JSON model:
```json
{
	"JWT": "yourJWTValue",
	"WithJWTObject": {
		"TweetId":"yourTweetId",
		"WithTweetObject": "Your comment"
	}
}
```

#### RatingAdding JSON model:
```json
{
	"JWT": "yourJWTValue",
	"WithJWTObject": {
		"TweetId":"yourTweetId",
		"WithTweetObject": "Your rateState: Dislike/None/Like"
	}
}
```