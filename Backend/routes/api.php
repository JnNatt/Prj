<?php

use Illuminate\Http\Request;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('jwt.auth')->get('/user', function (Request $request){
    return $request-> user();
});
Route::get('/users', 'Api\UsersController@index');
Route::get('/myUser', 'Api\UsersController@GetMyUser');

Route::post('login', 'Api\AuthController@login');
Route::post('register', 'Api\AuthController@register');

Route::get('timeline', 'Api\TimelineController@index');
Route::get('timeline/th', 'Api\TimelineController@GetTimelineTh');
Route::get('timeline/w', 'Api\TimelineController@GetTimelineW');
Route::get('timeline/category/{categoryId}', 'Api\TimelineController@GetTimelineByCategory');