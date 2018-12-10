<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AJAXProductList.ascx.cs"
    Inherits="eStore.UI.Modules.CertifiedPeripherals.AJAXProductList" %>
<div>
    <div class="epaps-row780">
        <div class="epaps-title-bgGray-borderLeft">
            <h1>
                Products</h1>
            <div class="caps-CurrentSelections">
                <div class="caps-title_add"   ng-click="isCollapsed = !isCollapsed" ng-init="isCollapsed=true"  ng-class="{'epaps-openBox': isCollapsed, 'openBox': !isCollapsed}">
                    Current Selections：({{selectedProducts.length}})
                </div>
                <ul class="caps-content_add" collapse="isCollapsed">
                    <li ng-repeat="pn in selectedProducts">
                        {{pn}}
                    </li>
                </ul>
            </div>
            <a ng-click="CompareProducts();">Compare</a>
            <div class="clearfix">
            </div>
        </div>
        <div class="epaps-productRow2" id="<%=ClientID %>2">
            <ul class="productlist">
                <li ng-repeat="product in pagedproducts">
                    <div class="epaps-productImg epaps-productImgwithborder">
                        <span class="epaps-{{product.PromotionType}}" ng-show="product.PromotionType!=''">
                        </span>
                        <div class="epaps-productlogo" ng-show="product.Manufacturer!=''">
                            <img src="https://wfcache.advantech.com/www/certified-peripherals/documents/LOGO/{{ product.Manufacturer }}.png" />
                        </div>
                        <a href="{{ product.Url }}">
                            <img src="{{ product.thumbnailImageX }}" width="166" height="166" />
                        </a>
                    </div>
                    <div class="epaps-productContent">
                        <div class="epaps-productLink epaps-immediate">
                            <a href="{{ product.Url }}">{{ product.name }}</a>
                        </div>
                        <div class="epaps-productTxt">
                            {{ product.productDescX }}
                        </div>
                        <div class="epaps-productprice" ng-bind-html="trustAsHtml(product.Price)">
                        </div>
                    </div>
                    <div class="epaps-compareBlock">
                        <input type="checkbox" ng-checked="selectedProducts.indexOf(product.SProductID) > -1"
                            ng-click="checkcomparisionitems($event,product)" /><label for="ckbcompare">Compare Product</label>
                    </div>
                </li>
            </ul>
            <div class="clearfix">
            </div>
            <div class="simple-pagination epaps-page" ng-show="showpagination">
                <pagination total-items="bigTotalItems" ng-model="bigCurrentPage" max-size="maxSize"
                    boundary-links="true" rotate="false" num-pages="numPages" items-per-page="itemsPerPage"
                    ng-change="pageChanged()"></pagination>
            </div>
            <div class="clearfix">
            </div>
        </div>
    </div>
</div>
<script type="text/javascript" language="javascript">
    var eStore = angular.module('eStore',['ui.bootstrap']);

    eStore.controller('eStoreProductsCtrl', function ($scope,$sce,$filter) {
        var products=<%=productsdata %>;
      
        $scope.products = products;
        $scope.trustAsHtml = function(html) {
            return $sce.trustAsHtml(html);
        };
        $scope.itemsPerPage = 8;
        $scope.currentPage = 1;

        $scope.maxSize = 5;
        $scope.bigTotalItems = $scope.products.length;
        $scope.bigCurrentPage = 1;
        $scope.totalItems = $scope.products.length;

        $scope.showpagination = false;

        $scope.selectedcategory = null;
        $scope.hassubcategories =angular.element(document.querySelector('#subcategories')).length==1;
        $scope.selectedProducts = [];

        $scope.speccriterica = function() {
    
            var allcriterica = [];
            angular.forEach($scope.products, function(prod) {
                angular.forEach(prod.specs, function(spec) {
                    if (allcriterica.length === 0) {

                        allcriterica.push(_.extend(spec, {
                            ProductId: prod.SProductID
                        }));
                    } else if (!_.any(allcriterica, function(c) {
                      c.ProductId == prod.SProductID && c.AttrName == spec.AttrName && c.AttrValueName == spec.AttrValueName
                    })) {
                        allcriterica.push(_.extend(spec, {
                            ProductId: prod.SProductID
                        }));
                    }
                });
            });

            var criterica = _
               .chain(allcriterica)
               .groupBy('AttrName')
               .map(function(value, key) {
                   return {
                       AttrName: key,
                       isCollapsed: true,
                       AttributeValues: _
                         .chain(value)
                         .countBy('AttrValueName')
                         .map(function(avalue, akey) {
                             return {
                                 AttrValueName: akey,
                                 Count: avalue,
                                 isChecked: false
                             }
                         })
                         .value()
                   }
               })
               .value();

            return criterica;
        };
        $scope.pagedproducts=[];
        $scope.init = function () {
            $scope.criterica = $scope.speccriterica();
            $scope.pagedproducts =$filter('productspaginationfilter')($scope.products, $scope.itemsPerPage,$scope.bigCurrentPage);
            $scope.showpagination =   $scope.products.length > $scope.itemsPerPage;
            
        }
        $scope.init();
        $scope.checkedcriterica = [];
        $scope.checkedcritericasummary=[];
        $scope.setcheckedcritericasummary=function()
        {
            $scope.checkedcritericasummary=_.chain($scope.checkedcriterica)
                    .groupBy('AttrName')
                    .map(function (avalue, akey) {
                        return {
                            AttrName: akey,
                            AttributeValues: avalue
                        }
                    })
                    .value();
        }
        $scope.resetCategory = function (id) {
            $scope.selectedcategory = id;
            $scope.products = $filter('producstcategoryfilter')(products, $scope.selectedcategory);
            $scope.criterica = $scope.speccriterica();
            $scope.checkedcriterica = [];
            $scope. checkedcritericasummary=[];
            $scope.selectedProducts = [];
            $scope.bigCurrentPage=1;
            $scope.bigTotalItems = $scope.products.length;
            $scope.showpagination =   $scope.products.length > $scope.itemsPerPage;
            $scope.pagedproducts =$filter('productspaginationfilter')($scope.products , $scope.itemsPerPage,$scope.bigCurrentPage);  
  
        }


        $scope.updatecriterica = function (AttrName,attributevalue)
        {
            if (!angular.isUndefined(attributevalue.isChecked)&&!attributevalue.isChecked)
            {
                $scope.checkedcriterica.push({ AttrName: AttrName ,AttrValueName:attributevalue.AttrValueName});
            }
            else
            {
                $scope.checkedcriterica = _.without($scope.checkedcriterica, _.findWhere($scope.checkedcriterica, { AttrName: AttrName, AttrValueName: attributevalue.AttrValueName }));
                if(angular.isUndefined(attributevalue.isChecked))
                {
                    var existscritericaname=  _.findWhere($scope.criterica, { AttrName: AttrName});
                    var existscritericavalue=  _.findWhere(existscritericaname.AttributeValues, { AttrValueName: attributevalue.AttrValueName });
                    
                    existscritericavalue.isChecked=false;
                }
            }
            $scope.setcheckedcritericasummary();
            var tmp= $filter('producstcategoryfilter')(products, $scope.selectedcategory);
            $scope.products =  $filter('producstspecfilter')(tmp, $scope.checkedcriterica);
            $scope.bigCurrentPage=1;
            $scope.bigTotalItems =  $scope.products.length;
            $scope.showpagination =    $scope.products.length > $scope.itemsPerPage;
            $scope.pagedproducts =$filter('productspaginationfilter')( $scope.products, $scope.itemsPerPage,$scope.bigCurrentPage);  
        }
        $scope.resetcriterica=function()
        {
            $scope.resetCategory (selectedcategory);
        }
        $scope.pageChanged=function(){
            $scope.pagedproducts =$filter('productspaginationfilter')($scope.products , $scope.itemsPerPage,$scope.bigCurrentPage);  
        }
        $scope.checkcomparisionitems=function(event,product)
        { 
            var idx = $scope.selectedProducts.indexOf(product.SProductID);
            if (idx > -1) {
                $scope.selectedProducts.splice(idx, 1);
            }
            else
            {
                if($scope.selectedProducts.length==4)
                {        
                    alert("You can only compare products upto 4 at once.");
                 event.preventDefault();
                }
                else
                {
                    $scope.selectedProducts.push(product.SProductID)
                }
            }
 
        }
        $scope.CompareProducts=function()
        {
            if($scope.selectedProducts.length>0)
            {
                window.open('/CertifiedPeripherals/Compare.aspx?parts=' + $scope.selectedProducts);
            }
            else
            {
                alert("Please select products to compare.");
            
            }

        }
    });

    eStore.filter('producstcategoryfilter', [
      function () {
          return function (products, selectedcategory) {

              if (!angular.isUndefined(products) && !angular.isUndefined(selectedcategory)&& selectedcategory!=null) {
                  var tmp = _.where(products, {
                      ProductCategoryId: parseInt( selectedcategory)
                  });
                  return tmp;
              } else {
                  return products;
              }
          };
      }
    ]);

    eStore.filter('productspaginationfilter', [
      function () {
          return function (products, itemsPerPage,bigCurrentPage) {
              var begin = ((bigCurrentPage - 1) * itemsPerPage) , end = begin + itemsPerPage;
              return products.slice(begin, end);
          };
      }
    ]);
    eStore.filter('producstspecfilter', [
      function () {
          return function (products, criterica) {

              if (!angular.isUndefined(products) && !angular.isUndefined(criterica) && criterica.length > 0) {
                  var specfilter = _
                .chain(criterica)
                .groupBy('AttrName')
                .map(function (avalue, akey) {
                    return {
                        AttrName: akey,
                        AttributeValues: avalue
                    }
                })
                .value();
                  var tmp = [];
                  angular.forEach(products, function (prod) {
                      var ismatched = true;
                      angular.forEach(specfilter, function (sf) {
                          if (ismatched) {
                              if (!angular.isUndefined(_.findWhere(prod.specs, { AttrName: sf.AttrName }))) {
                                  var ismatchedanyvalue=false;
                                  angular.forEach(sf.AttributeValues, function (sfv) {
                                      if (!ismatchedanyvalue) {
                                          if (!angular.isUndefined(_.findWhere(prod.specs, { AttrName: sf.AttrName, AttrValueName: sfv.AttrValueName })))
                                          {
                                              ismatchedanyvalue = true;
                                          }
                                     
                                      }
                                  });
                                  if (!ismatchedanyvalue)
                                  {
                                      ismatched = false;
                                  }
                              }
                              else {
                                  ismatched = false;
                              }
                          }
                      });
                      if (ismatched)
                      {
                          tmp.push(prod);
                      }
               
                  });
             
                  return tmp;
          
              } else {
                  return products;
              }
          };
      }
    ]);
</script>
