using System.Collections.Generic;

namespace Lis.Test.Integration.Common
{
    public class Systems
    {
        #region Systems

        public const string PATIENT_PASSPORT = "urn:oid:1.2.643.5.1.34";
        public const string PATIENT_SNILS = "urn:oid:1.2.643.3.9";
        public const string MENOPAUSE = "urn:oid:1.2.643.2.69.1.1.1.39";
        public const string DATA_ABSENT_REASON = "urn:oid:1.2.643.2.69.1.1.1.38";
        public static string ORGANIZATIONS = "urn:oid:1.2.643.2.69.1.2";

        public const string INSURANCE_ORGANIZATIONS = "urn:oid:1.2.643.5.1.13.2.1.1.635";

        public const string OBSERVATION_LOINC = "urn:oid:1.2.643.2.69.1.1.1.1";

        /// <summary>
        /// Код МЭС
        /// </summary>
        public const string MES = "urn:oid:1.2.643.2.69.1.1.1.61";

        /// <summary>
        /// Код МКБ10
        /// </summary>
        public const string MKB10 = "urn:oid:1.2.643.2.69.1.1.1.2";

        /// <summary>
        /// Код биоматериала
        /// </summary>
        public const string BIOMATERIAL = "urn:oid:1.2.643.2.69.1.1.1.33";

        /// <summary>
        /// Тип контейнера
        /// </summary>
        public const string CONTAINER_TYPE = "urn:oid:1.2.643.2.69.1.1.1.34";

        /// <summary>
        /// Тип случая обслуживания
        /// </summary>
        public const string ENCOUNTER_TYPE = "urn:oid:1.2.643.2.69.1.1.1.35";

        /// <summary>
        /// Тип заболевания
        /// </summary>
        public const string CONDITION_CATEGORY = "urn:oid:1.2.643.2.69.1.1.1.36";

        /// <summary>
        /// Код Observation
        /// </summary>
        public const string OBSERVATION_NAME = "urn:oid:1.2.643.2.69.1.1.1.37";

        /// <summary>
        /// Код должности врача
        /// </summary>
        public const string PRACTITIONER_ROLE = "urn:oid:1.2.643.5.1.13.2.1.1.607";

        /// <summary>
        /// Код специальности врача
        /// </summary>
        public const string PRACTITIONER_SPECIALITY = "urn:oid:1.2.643.5.1.13.2.1.1.181";

        /// <summary>
        /// Тип страхового случая
        /// </summary>
        public const string COVERAGE_TYPE = "urn:oid:1.2.643.2.69.1.1.1.48";

        /// <summary>
        /// Цель посещения
        /// </summary>
        public const string REASON_CODE = "urn:oid:1.2.643.2.69.1.1.1.19";

        /// <summary>
        /// Отметка срочности
        /// </summary>
        public const string PRIORIRY = "urn:oid:1.2.643.2.69.1.1.1.30";

        /// <summary>
        /// Код услуги заявки
        /// </summary>
        public const string DIAGNOSTIC_ORDER_CODE = "urn:oid:1.2.643.2.69.1.1.1.31";

        /// <summary>
        /// Код источника финансирования
        /// </summary>
        public const string FINANCIAL = "urn:oid:1.2.643.2.69.1.1.1.32";

        /// <summary>
        /// Номенклатура медицинских услуг, 1.2.643.5.1.13.2.1.1.473
        /// </summary>
        public const string DIAGNOSTIC_REPORT_CODE = "urn:oid:1.2.643.2.69.1.1.1.31";

        #endregion

        #region Extensions

        /// <summary>
        /// Расширение "источник финансирования" для DiagnosticOrder
        /// </summary>
        public const string DIAGNOSTIC_ORDER_FINANCIAL_EXTENSION = "urn:oid:1.2.643.2.69.1.100.1";

        /// <summary>
        /// Расширение "Данные о страховке" для DiagnosticOrder
        /// </summary>
        public const string DIAGNOSTIC_ORDER_INSURANCE_EXTENSION = "urn:oid:1.2.643.2.69.1.100.2";

        #endregion
    }
}

