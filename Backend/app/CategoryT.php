<?php
/**
 * Created by PhpStorm.
 * User: Wasenshi
 * Date: 2/24/2018
 * Time: 10:43 PM
 */

namespace App;


use Illuminate\Database\Eloquent\Model;

class CategoryT extends Model
{
    protected $table = 'lesson';
    protected $primaryKey = 'id_category_lesson';

    protected $appends = ['category_id', 'name', 'timeline'];
    protected $visible = ['category_id', 'name', 'timeline'];

    public function getCategoryIdAttribute(){
        return $this->attributes[$this->primaryKey];
    }
    public function getNameAttribute(){
        return $this->attributes['category_lesson'];
    }
    public function setNameAttribute($value){
        return $this->attributes['category_lesson'] = $value;
    }
    public function getTimelineAttribute(){
        return $this->timelineW()->get()->concat($this->timelineTh()->get())->sortBy('order')->values();
    }

    public function timelineTh(){
        return $this->hasMany(TimePointThai::class, TimePointThai::$category);
    }

    public function timelineW(){
        return $this->hasMany(TimePointWest::class, TimePointWest::$category);
    }
}