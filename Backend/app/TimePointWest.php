<?php

namespace App;

class TimePointWest extends TimePoint
{
    public $type = "w";

    protected $table = self::table;
    protected $primaryKey = self::primaryKey;

    const table = 'timeline_w';
    const primaryKey = 'idtimelineW_Data';
    public static $category = 'category_w';
    protected static $year_title = 'year_whistory';
    protected static $description = 'lesson_descript_w';
    /**
     * The attributes that are mass assignable.
     *
     * @var array
     */
    protected $fillable = [
        'category_w', 'order', 'year_whistory', 'lesson_descript_w'
    ];
}
