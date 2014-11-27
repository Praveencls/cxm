(function (angular) {

   

    'use strict';

    angular.module('controllerAsExample', [])

      .controller('CallWebApi', ['$scope', CallWebApi]);

  

    function CallWebApi($scope) {

      

        $scope.mydata = [{

            Category: 'Content',

            Database: 'master',

            DisplayName: 'Home',

            HasChildren: true,

            Icon: '/temp/IconCache/Network/16x16/home.png',

            ID: '{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}'

            }];

 

    $.ajax({

        url: "http://dms72/-/item/v1/?scope=s&sc_itemid={110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}"

    }).then(function (data) {

          

        $scope.data = data.result.items;

        //alert($scope.data.result.items[0].Name);

        // alert(data.result.items[0].Name + " in");

    });

 

      

}

 

})(window.angular);