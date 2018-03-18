<?php

namespace App\Http\Controllers\Api;

use App\RegisterFactory;
use App\User;
use Illuminate\Auth\Events\Registered;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;
use Tymon\JWTAuth\Exceptions\JWTException;
use Tymon\JWTAuth\Facades\JWTAuth;

class AuthController extends Controller
{
    public function register(Request $request)
    {
        $data = $request->all();
        $validator = RegisterFactory::Validator($data, false);
        if($validator->fails()){
            return response()->json(['success' => false, 'error' => 'There is some error. Please see detail in the data.', 'data' => $validator->errors()]);
        }
        $user = RegisterFactory::CreateUser($data);
        event(new Registered($user));

        return response()->json(['success' => true]);
    }

    /**
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function login(Request $request)
    {
        $params = $request->only('username', 'password');

        if(sizeof($params) != 2){
            return response()->json(['success' => false, 'error' => 'Please provide username and password.'], 400);
        }

        $username = $params['username'];
        $password = $params['password'];

        $credentials = [
            'username' => $username,
            'password' => $password,
        ];

        try {
            // attempt to verify the credentials and create a token for the user
            if (! $token = JWTAuth::attempt($credentials)) {
                return response()->json(['success' => false, 'error' => 'Invalid Credentials. Please make sure you entered the right username and password.'], 401);
            }
        } catch (JWTException $e) {
            // something went wrong whilst attempting to encode the token
            return response()->json(['success' => false, 'error' => 'could_not_create_token'], 500);
        }

        // all good so return the token
        return response()->json(['success' => true, 'data'=> [ 'token' => $token ]]);
    }

    public function loginUser(User $user){
        return JWTAuth::fromUser($user);
    }

    public function logout(Request $request)
    {
        $this->validate($request, ['token' => 'required']);
        try {
            JWTAuth::invalidate($request->input('token'));
            return response()->json(['success' => true]);
        } catch (JWTException $e) {
            // something went wrong whilst attempting to encode the token
            return response()->json(['success' => false, 'error' => 'Failed to logout, please try again.'], 500);
        }
    }
}
