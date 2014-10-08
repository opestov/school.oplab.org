'use strict'

var gulp = require('gulp'),
    concat = require('gulp-concat'),
    cssmin = require('gulp-minify-css'),
    htmlmin = require('gulp-htmlmin');

gulp.task('css', function() {
    gulp.src(['./intermediate/css/normalize.css', './intermediate/css/*.css'])
        .pipe(concat('min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('build/css'));
});

gulp.task('html', function() {
    gulp.src('./intermediate/**/*.html')
        .pipe(htmlmin({"collapseWhitespace": true, "conservative-collapse": true}))
        .pipe(gulp.dest('build'));
});

gulp.task('default', ['css', 'html']);