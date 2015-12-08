var ExplorerApp = angular.module("ExplorerApp", ["ngResource", "ngRoute"]).
    config(function ($routeProvider) {
        $routeProvider.
            when('/', { controller: ListCtrl, templateUrl: 'list.html' }).
            otherwise({ redirectTo: '/' });
    });

ExplorerApp.factory('Subdirectories', function ($resource) {
    return $resource('api/Directory/Get');
});

var ListCtrl = function ($scope, $location, Subdirectories) {
    $scope.send_back = function (pat) {
        if (pat === null) {
            $scope.items = Subdirectories.get({ path: "", is_back: true });
        }
        else {
            $scope.items = Subdirectories.get({ path: pat, is_back: true });
        }
    }

    $scope.items = Subdirectories.get();

    $scope.send_path = function (pat) {
        $scope.items = Subdirectories.get({ path: pat, is_back: false });
    };
};