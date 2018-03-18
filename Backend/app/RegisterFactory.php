<?php
/**
 * Created by PhpStorm.
 * User: Wasenshi
 * Date: 2/20/2018
 * Time: 10:26 PM
 */

namespace App;


use Illuminate\Support\Facades\Validator;

class RegisterFactory
{
    public static function CreateUser(array $data){
        return User::create([
            'username' => $data['username'],
            'email' => $data['email'],
            'password' => bcrypt($data['password'])
        ]);
    }

    public static function Validator(array $data, bool $confirmPassword = true){
        return Validator::make($data, [
            'username' => 'required|string|max:45|unique:users',
            'email' => 'required|string|email|max:45|unique:users',
            'password' => 'required|string|min:6'.($confirmPassword?'|confirmed':''),
        ]);
    }
}