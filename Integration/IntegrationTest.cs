using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Lis.Test.Integration.Common;
using Monads.NET;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Lis.Test.Integration
{
    [TestFixture]
    class IntegrationTest
    {
        #region Infrastructure

        public List<Bundle> Orders { get; set; }

        public FhirClient FhirClient { get; set; }

        public IntegrationTest()
        {
            Orders = new List<Bundle>();
            for (var i = 0; i < 1; i++)
            {
                Orders.Add(IntegrationHelper.CreateRandomLabOrder());
            }

            FhirClient = new FhirClient(Constants.Endpoint);
        }

        [TestFixtureSetUp]
        public void Init()
        {
            LogHelper.Clear();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            //FhirResourceHelper.ClearTables();
        }

        #endregion

        [TestCaseSource("Orders")]
        public void CreateOrder_And_Send_OrderResult(Bundle lisOrder)
        {
            LogHelper.SaveOrderBundle(lisOrder, Guid.NewGuid().ToString());

            // Отправляем заявку на исследование
            var orderResponse = FhirClient.Transaction(lisOrder);

            // Проверяем, что заказ создался на сервере
            ValidationHelper.ValidateLisOrderResponse(orderResponse);

            //Получаем пробирку со штрихкодом по параметрам
            var order = IntegrationHelper.GetOrder(orderResponse);
            
            // Создаем результат лабораторного исследования для заказа 
            var labResult = IntegrationHelper.CreateLabResultForOrder(order);

            LogHelper.SaveResultBundle(labResult, Guid.NewGuid().ToString());

            // Отправляем результат на сервер
            var labResultResponse = FhirClient.Transaction(labResult);

            // Проверяем, что результат создался на сервере
            ValidationHelper.ValidateLabResultResponse(labResultResponse);
        }

        [TestCaseSource("Orders")]
        public void Get_Order_By_TargetCode_And_Barcode(Bundle lisOrder)
        {
            //Создаем заказ
            var orderResponse = FhirClient.Transaction(lisOrder);

            var validTargetCodesAndBarcodes = IntegrationHelper.GetOrderCodes(orderResponse);

            foreach (var validTargetCodesAndBarcode in validTargetCodesAndBarcodes)
            {
                var response = FhirResourceHelper.GetOrderOperation(
                    targetCode: validTargetCodesAndBarcode.Item1,
                    barcode: validTargetCodesAndBarcode.Item2);

                Assert.That(response, Is.Not.Null);
                Assert.That(response.Parameter, Is.Not.Null);
                Assert.That(response.Parameter.Count, Is.EqualTo(1));

                var bundleOrder = orderResponse
                    .With(x => x.Entry)
                    .With(x => x.FirstOrDefault(y => y.Resource is Order));

                Assert.That(bundleOrder, Is.Not.Null);
                Assert.That(bundleOrder.Resource, Is.Not.Null);

                var order = response
                    .With(x => x.Parameter)
                    .With(x => x.FirstOrDefault(y => y.Resource is Order));

                Assert.That(order, Is.Not.Null);
                Assert.That(order.Resource, Is.Not.Null);
                Assert.That(order.Resource.Id, Is.EqualTo(bundleOrder.Resource.Id));
            }
        }

        [TestCaseSource("Orders")]
        public void Get_Result_By_TargetCode_And_Barcode(Bundle lisOrder)
        {
            // Отправляем заявку на исследование
            var orderResponse = FhirClient.Transaction(lisOrder);

            ValidationHelper.ValidateLisOrderResponse(orderResponse);

            // Запрашиваем заказ по barcode и targecode
            var orderCodes = IntegrationHelper.GetOrderCodes(orderResponse).First();
            var getOrderResponse = FhirResourceHelper.GetOrderOperation(targetCode: orderCodes.Item1,
                    barcode: orderCodes.Item2);
            var order = (Order)getOrderResponse.Parameter.Find(x => x.Name == "Order").Resource;

            // Создаем результат лабораторного исследования для заказа 
            var labResult = IntegrationHelper.CreateLabResultForOrder(order);

            // Отправляем результат на сервер
            var labResultResponse = FhirClient.Transaction(labResult);

            ValidationHelper.ValidateLabResultResponse(labResultResponse);

            // Запрашиваем результат по OrderMisID и targecode
            var resultCode = IntegrationHelper.GetResultCodes(order);
            var resultRespose = FhirResourceHelper.GetResultOperation(targetCode: resultCode.Item1,
                    orderMisId: resultCode.Item2);

            ValidationHelper.ValidateLabResultOrderResponse(resultRespose);
        }
    }
}
