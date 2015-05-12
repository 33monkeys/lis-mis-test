namespace Lis.Test.Integration.Common
{
    public class Systems
    {
        #region Systems

        public const string OBSERVATION_LOINC = "http://netrika.ru/observation-loinc";

        /// <summary>
        /// Идентификатор заявки в МИС
        /// </summary>
        public const string MIS_IDENTIFIER = "http://netrika.ru/mis-identifier";

        /// <summary>
        /// Идентификатор передающей системы
        /// </summary>
        public const string MIS = "http://netrika.ru/mis";

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public const string ORG = "http://netrika.ru/organization";

        /// <summary>
        /// Код МЭС
        /// </summary>
        public const string MES = "http://netrika.ru/mes";

        /// <summary>
        /// Код МКБ10
        /// </summary>
        public const string MKB10 = "http://netrika.ru/mkb10";

        /// <summary>
        /// Код биоматериала
        /// </summary>
        public const string BIOMATERIAL = "http://netrika.ru/biomaterial";

        /// <summary>
        /// Тип контейнера
        /// </summary>
        public const string CONTAINER_TYPE = "http://netrika.ru/container-type";

        /// <summary>
        /// Штрих-код контейнера с биоматериалом
        /// </summary>
        public const string CONTAINER_TYPE_IDENTIFIER = "http://netrika.ru/container-type-identifier";

        /// <summary>
        /// Тип случая обслуживания
        /// </summary>
        public const string ENCOUNTER_TYPE = "http://netrika.ru/encounter-type";

        /// <summary>
        /// Тип заболевания
        /// </summary>
        public const string CONDITION_CATEGORY = "http://netrika.ru/condition-category";

        /// <summary>
        /// Код Observation
        /// </summary>
        public const string OBSERVATION_NAME = "http://netrika.ru/observation-name";

        /// <summary>
        /// Код должности врача
        /// </summary>
        public const string PRACTITIONER_ROLE = "http://netrika.ru/practitioner-role";

        /// <summary>
        /// Код специальности врача
        /// </summary>
        public const string PRACTITIONER_SPECIALITY = "http://netrika.ru/practitioner-speciality";

        /// <summary>
        /// Тип страхового случая
        /// </summary>
        public const string COVERAGE_TYPE = "http://netrika.ru/coverage-type";

        /// <summary>
        /// Цель посещения
        /// </summary>
        public const string REASON_CODE = "http://netrika.ru/reason-code";

        /// <summary>
        /// Отметка срочности
        /// </summary>
        public const string PRIORIRY = "http://netrika.ru/priority";

        /// <summary>
        /// Код услуги заявки
        /// </summary>
        public const string DIAGNOSTIC_ORDER_CODE = "http://netrika.ru/diagnostic-order-code";

        /// <summary>
        /// Код источника финансирования
        /// </summary>
        public const string FINANCIAL = "http://netrika.ru/financial";

        /// <summary>
        /// Идентификатор врача
        /// </summary>
        public const string PRACTITIONER_IDENTIFIER = "http://netrika.ru/practitioner-identifier";

        /// <summary>
        /// Номенклатура медицинских услуг, 1.2.643.5.1.13.2.1.1.473
        /// </summary>
        public const string DIAGNOSTIC_REPORT_CODE = "http://netrika.ru/diagnostic-report-code";

        /// <summary>
        /// Методика исследования.
        /// </summary>
        public const string OBSERVATION_METHOD = "http://netrika.ru/observation-method";

        #endregion

        #region Extensions

        /// <summary>
        /// Расширение "источник финансирования" для DiagnosticOrder
        /// </summary>
        public const string DIAGNOSTIC_ORDER_FINANCIAL_EXTENSION = "http://netrika.ru/diagnostic-order-financial-extension";

        /// <summary>
        /// Расширение "Данные о страховке" для DiagnosticOrder
        /// </summary>
        public const string DIAGNOSTIC_ORDER_INSURANCE_EXTENSION = "http://netrika.ru/diagnostic-order-insurance-extension";

        #endregion
    }
}

