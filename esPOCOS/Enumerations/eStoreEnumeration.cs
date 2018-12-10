using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.Enumerations
{
   public static class eStoreConfigure
    {
      public  enum Currency { AUD,EUR,INR,KRW,NTD,RM,RMB,SGD,THB,USD,YEN };
      public  enum DMF {AAUDMF,ACNDMF,AEUDMF,AINDMF,AJPDMF,AKRDMF,ANADMF,ATHDMF,ATWDMF,EMTDMF,SAPDMF};
      public  enum AddressType { Billing,Shipping,Dropship,ShipFrom };

      public enum RewardLogTransactionType
      {
          Credit_Back = 2,//礼品还没有发货,  用户要求退换 礼品.
          Approved = 1,//同意订单积分  (Order)
          Pending = 0,//等待审核  (Order)
          Redeemed = -1,//申请兑换礼品
          Returned_Order = -2,//礼品发送了, 但是用户退Order
          Shipped = -3,//发送礼品
          Rejected = -4,//拒绝订单积分 (Order)
          Cancel = -5 //取消礼品   -1的时候,变成Credit_Back,就是-5
      }

      public enum MyAccountCSS { on};
    }
}
