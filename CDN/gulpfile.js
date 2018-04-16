var gulp = require("gulp");
var del = require('del');
var rename = require('gulp-rename');
var uglify = require('gulp-uglify');
var cleancss = require('gulp-clean-css');
var concat = require('gulp-concat');
var flatten = require('gulp-flatten');
var expect = require('gulp-expect-file');

var supportedfonts = [
    'node_modules/**/fonts/*.woff',
    'node_modules/**/fonts/*.woff2',
    'node_modules/**/fonts/*.eot',
    'node_modules/**/fonts/*.svg',
    'node_modules/**/fonts/*.ttf',
    'node_modules/**/fonts/*.otf'
];

var packages = [
    // AiurCore - CSS
    {
        inputFiles: [
            'node_modules/bootstrap/dist/css/bootstrap.css',
            'node_modules/font-awesome/css/font-awesome.css',
            'css/AiurCore.css'
        ],
        iscss: true,
        outputFileName: 'AiurCore.min.css'
    },
    // AiurCore - JS
    {
        inputFiles: [
            'node_modules/jquery/dist/jquery.js',
            'node_modules/popper.js/dist/umd/popper.min.js',
            'node_modules/bootstrap/dist/js/bootstrap.js',
            'node_modules/jquery-validation/dist/jquery.validate.js',
            'node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js',
            'node_modules/clipboard/dist/clipboard.min.js',
            'node_modules/jquery-disable-with/src/jquery-disable-with.js',
            'js/AiurCore.js'
        ],
        iscss: false,
        outputFileName: 'AiurCore.min.js'
    },
    // AiurMarket - CSS
    {
        inputFiles: [
            'css/AiurMarket.css'
        ],
        iscss: true,
        outputFileName: 'AiurMarket.min.css'
    },
    // AiurMarket - JS
    {
        inputFiles: [
            'js/AiurMarket.js'
        ],
        iscss: false,
        outputFileName: 'AiurMarket.min.js'
    },
    // AiurProduct - CSS
    {
        inputFiles: [
            'node_modules/primer/build/build.css',
            'css/AiurProduct.css'
        ],
        iscss: true,
        outputFileName: 'AiurProduct.min.css'
    },
    // AiurProduct - JS
    {
        inputFiles: [
            'js/AiurProduct.js'
        ],
        iscss: false,
        outputFileName: 'AiurProduct.min.js'
    },
    // AiurDashboard - CSS
    {
        inputFiles: [
            'node_modules/startbootstrap-sb-admin/css/sb-admin.css',
            'node_modules/datatables/media/css/jquery.dataTables.css',
            'node_modules/primer-markdown/build/build.css',
            'css/AiurDashboard.css'
        ],
        iscss: true,
        outputFileName: 'AiurDashboard.min.css'
    },
    // AiurDashboard - JS
    {
        inputFiles: [
            'node_modules/startbootstrap-sb-admin/js/sb-admin.js',
            'node_modules/datatables/media/js/jquery.dataTables.js',
            'js/AiurDashboard.js'
        ],
        iscss: false,
        outputFileName: 'AiurDashboard.min.js'
    },
]

gulp.task('clean', function () {
    del('dist/**/*');
    del('fonts/**/*');
})

gulp.task("bundle", function () {
    packages.forEach(function (package) {
        gulp.src(supportedfonts)
            .pipe(flatten())
            .pipe(gulp.dest('fonts'));
        if (package.iscss) {
            gulp.src(package.inputFiles)
                .pipe(expect(package.inputFiles))
                .pipe(concat('temp'))
                .pipe(cleancss())
                .pipe(rename(package.outputFileName))
                .pipe(gulp.dest('dist'));
        } else {
            gulp.src(package.inputFiles)
                .pipe(expect(package.inputFiles))
                .pipe(concat('temp'))
                .pipe(uglify())
                .pipe(rename(package.outputFileName))
                .pipe(gulp.dest('dist'));
        }
    });
});