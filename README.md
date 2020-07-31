# BelTwit-REST-API


## About the API
> This is API that gives you a possibility to manipulate with tweets (some blocks of news, that users post). The idea was to create a simulation of popular social network __twitter__. 
The API consists of __3 controllers__: 
* [UserController](#usercontroller) - create, modify and delete users. Subscribe/unsubscribe + getSubscriptions/Subscribers. 
* [AuthController](#authcontroller) - authentificate users (by JWT) + updating tokens
* [TwitterController](#twittercontroller) - create and delete tweets. Comment, rate(likes/dislikes), retweet. Get tweets by TweetId/UserLogin

## Special features of the project
- [x] __3__ controllers, __40__ methods, __9__ request bodies
- [x] __HEAD__ and __OPTIONS__ methods realization
- [x] Models for requests bodies realization with __T-type__ and __Tuples__ 
- [x] Authentication of the user with __JWT__ (created __manually__ without any libraries)
- [x] __2 tokens__ authentication (JWT + __RefreshTokens__ in database)
- [x] Secure storing passwords (with SHA512 hash)
- [x] __Admin user role__ is available
- [x] Sophisticated work with the __database__ (many-to-many, one-to-many relationships)
- [x] __Self-referencing many-to-many__ connection (for subscribing system)
- [x] __Logging__ of all actions (and writing down them to database)
- [x] Beautiful and convenient __documentation__
- [x] All requests are ready for you to test in __Postman__ - (see `Postman_collection.json` file)

## UserController(16 methods)
| Method    | URL | Body | Description | Status codes |
| :-------: | :-- | :--- | :---------- | :----------- |
|__OPTIONS__| api/user 			   	  |||`200`
| HEAD   | api/user 			      |||`200`
| GET    | api/user     		      || Get all the users.|`200`
| POST   | api/user        	 	      |[User model](#user-json-model)| Create a single user. Login(unique, length: [2;20]) and password(length: [5;100]) are required.|`200`, `403`, `404`
| PUT    | api/user       	 	      |[2 users tuple](#2-users-json-tuple)| Update the user by changing property values. Item1 represents OldUser(old login and password are required), and Item2 representes NewUser(all changes are here).|`200`, `403`, `404`
| DELETE | api/user        	 	      |[User model](#user-json-model)	  | Deletes the user(password and login are required).|`200`, `403`, `404`
|__OPTIONS__| api/admin-delete 	      |||`200`
| DELETE | api/user/admin-delete      |[JwtWithUserId model](#jwtwithid-json-model)| Deletes the user by Id. Only for admnistrators!|`200`, `400`, `403`, `404`
|__OPTIONS__| api/user/subscribe      |||`200`
| HEAD   | api/user/subscribe 	   	  |`"YourJWTValue"`||`200`, `400`, `404`
| GET 	 | api/user/subscribe		  |`"YourJWTValue"`| Get your subscriptions(on whom you subscribed) by JWT value.|`200`, `400`, `404`
| POST   | api/user/subscribe      	  |[UserSubscribe model](#usersubscribe/unsubscribe-json-model)| Subscribes on the other user by your own JWT value and login of the other user.|`200`, `400`, `404`
| DELETE | api/user/subscribe      	  |[UserUnsubscribe model](#usersubscribe/unsubscribe-json-model)| Unsubscribes from the other user by your own JWT value and login of the other user.|`200`, `400`, `404`
|__OPTIONS__| api/user/get-subscribers|||`200`
| HEAD   | api/user/get-subscribers	  |`"YourJWTValue"`||`200`, `400`, `404`
| GET    | api/user/get-subscribers	  |`"YourJWTValue"`| Get your subscribers(who subscribed on you) by JWT value.|`200`, `400`, `404`





## AuthController(6 methods)
| Method    | URL | Body | Description | Status codes |
| :-------: | :-- | :--- | :---------- | :----------- |
|__OPTIONS__| api/auth 			 	|||`200`
| HEAD   | api/auth 			 	|`"YourJWTValue"`||`200`, `400`
| GET    | api/auth     	 	 	|`"YourJWTValue"`| Authorize the user by JWT value as a string.|`200`, `400`
| POST   | api/auth	 			 	|[User model](#user-json-model)| Authentificate the user by creating JWT(30 min) and RefreshToken(60 days). You can't have more than 5 RefreshTokens (or others will be deleted)|`200`, `403`, `404`
|__OPTIONS__| api/auth/update-tokens|||`200`
| POST   | api/auth/update-tokens	|[AccessRefreshToken model](#accessrefreshtoken-json-model)  | Refresh your JWT[or AccessToken] (for 30 minutes) and your RefreshToken (for 60 days).|`200`, `400`



## TwitterController(18 methods)
| Method    | URL | Body | Description | Status codes |
| :-------: | :-- | :--- | :---------- | :----------- |
|__OPTIONS__| api/twitter/getByLogin/{login}||||`200`
| HEAD   | api/twitter/getByLogin/{login}||||`200`, `404`
| GET    | api/twitter/getByLogin/{login}|| Get all tweets of user with "login".|`200`, `404`
|__OPTIONS__| api/twitter/getById/{id} 	 |||`200`
| HEAD   | api/twitter/getById/{id} 	 |||`200`, `400`, `404`
| GET    | api/twitter/getById/{id}      || Get tweet by "id".|`200`, `400`, `404`
|__OPTIONS__| api/twitter 				 |||`200`
| HEAD   | api/twitter 					 |`"YourJWTValue"`||`200`, `400`, `404`
| GET    | api/twitter  				 |`"YourJWTValue"`| Get tweets of your subscriptions(users on whom you've subscribed). JWT required.|`200`, `400`, `404`
| POST   | api/twitter                   |[JwtWithTweet model](#jwtwithtweet-json-model)  | Creates a user tweet. JWT and Tweet (Content at least) required.|`200`, `400`, `404`
| DELETE | api/twitter   				 |[JwtWithTweetId model](#jwtwithid-json-model)   | Deletes a user tweet. JWT and TweetId required. Admin can delte any tweet.|`200`, `400`, `404`
|__OPTIONS__| api/twitter/comment-tweet  |||`200`
| POST   | api/twitter/comment-tweet	 |[CommentAdding model](#commentadding-json-model)| Comments selected tweet(by TweetId). JWT required.|`200`, `400`, `404`
| DELETE | api/twitter/comment-tweet	 |[JwtWithCommentId model](#jwtwithid-json-model) | Delete comment by Id. JWT and TweetId required. Admin can delte any comment.|`200`, `400`, `404`
|__OPTIONS__| api/twitter/rate-tweet   	 |||`200`
| PUT    | api/twitter/rate-tweet		 |[RatingAdding model](#ratingadding-json-model)  | Rates selected tweet(by TweetId). There are 3 possible rates `"Dislike"`,`"None"`,`"Like"`. JWT required.|`200`, `400`, `404`
|__OPTIONS__| api/twitter/retweet   	 |||`200`
| POST   | api/twitter/retweet	 		 |[JwtWithTweetId model](#jwtwithid-json-model)	  | Retweet selected tweet to your user. JWT required.|`200`, `400`, `404`







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
