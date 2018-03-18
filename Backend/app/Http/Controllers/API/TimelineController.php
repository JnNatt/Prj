<?php

namespace App\Http\Controllers\Api;

use App\CategoryT;
use App\TimePointThai;
use App\TimePointWest;
use App\Http\Controllers\Controller;


class TimelineController extends Controller
{

    public function index(){
        $cat = CategoryT::with('timelineTh', 'timelineW')->get();

        return $cat;
    }

    public function GetTimelineW(){
        $timepoints = TimePointWest::all();

        return $timepoints->sortBy(function($item) {
            return $item->category.$item->order;
        })->values();
    }

    public function GetTimelineTh(){
        $timepoints = TimePointThai::all();

        return $timepoints->sortBy(function($item) {
            return $item->category.$item->order;
        })->values();
    }

    public function GetTimelineByCategory($categoryId){
        $timepoints = TimePointThai::category($categoryId)->get()->concat(TimePointWest::category($categoryId)->get());

        return $timepoints->sortBy('order')->values()->makeHidden('category');
    }
}
