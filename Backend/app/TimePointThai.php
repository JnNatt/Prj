<?php

namespace App;

class TimePointThai extends TimePoint
{
    public $type = "th";

    protected $table = self::table;
    protected $primaryKey = self::primaryKey;

    const table = 'timeline_th';
    const primaryKey = 'idtimeline_Data';
    public static $category = 'category_th';
    protected static $year_title = 'year_thaihistory';
    protected static $description = 'lesson_descript_thai';
    /**
     * The attributes that are mass assignable.
     *
     * @var array
     */
    protected $fillable = [
        'category_th', 'order', 'year_thaihistory', 'lesson_descript_thai'
    ];


}
