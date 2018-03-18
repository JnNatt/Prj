<?php

namespace App;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Notifications\Notifiable;

abstract class TimePoint extends Model
{
    use Notifiable;

    public $type;

    protected static $category;
    protected static $order = 'order';
    protected static $year_title;
    protected static $description;

    protected $visible = ['type', 'category', 'order', 'year_title', 'description'];
    protected $appends = ['type', 'category', 'order', 'year_title', 'description'];

    public function getTypeAttribute()
    {
        return $this->type;
    }
    public function getCategoryAttribute(){
        return $this->attributes[$this::$category];
    }
    public function setCategoryAttribute($value){
        return $this->attributes[$this::$category] = $value;
    }
    public function getOrderAttribute(){
        return $this->attributes[$this::$order];
    }
    public function setOrderAttribute($value){
        return $this->attributes[$this::$order] = $value;
    }
    public function getYearTitleAttribute(){
        return $this->attributes[$this::$year_title];
    }
    public function setYearTitleAttribute($value){
        return $this->attributes[$this::$year_title] = $value;
    }
    public function getDescriptionAttribute(){
        return $this->attributes[$this::$description];
    }
    public function setDescriptionAttribute($value){
        return $this->attributes[$this::$description] = $value;
    }

    /*public function fill(array $attributes)
    {
        $this->attributes[$this->category] = $this->appends['category'];
        $this->attributes[$this->year_title] = $this->appends['year_title'];
        $this->attributes[$this->description] = $this->appends['description'];

        return parent::fill($attributes);
    }*/

    public function scopeCategory($query, $id)
    {
        return $query->where($this::$category, $id);
    }
}
