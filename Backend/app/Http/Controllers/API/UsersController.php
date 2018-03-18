<?php

namespace App\Http\Controllers\Api;

use App\User;
use Illuminate\Http\Request;
use App\Http\Controllers\Controller;
use Illuminate\Support\Facades\Auth;


class UsersController extends Controller
{
    /**
     * Create a new controller instance.
     *
     * @return void
     */
    public function __construct()
    {
        $this->middleware('jwt.auth');
    }

    public function index(){
        $users = User::all();

        return $users;
    }

    public function GetMyUser(){
        return response()->json(User::where('username', Auth::guard('api')->User()->username)->get());
    }
}
