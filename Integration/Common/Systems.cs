using System.Collections.Generic;

namespace Lis.Test.Integration.Common
{
    public class Systems
    {
        #region Systems

        public static string PATIENT_PASSPORT = "urn:uuid:1.2.643.5.1.34";
        public static string PATIENT_SNILS = "urn:uuid:1.2.643.5.1.pfr";
        public static string MENOPAUSE = "urn:uuid:1.2.643.2.69.1.1.1.39";
        public static string DATA_ABSENT_REASON = "urn:uuid:1.2.643.2.69.1.1.1.38";
        public static List<string> ORGANIZATIONS = new List<string>
        {
            "urn:uuid:1.2.643.5.1.100",
            "urn:uuid:1.2.643.5.1.101",
            "urn:uuid:1.2.643.5.1.102",
        };

        public static string INSURANCE_ORGANIZATIONS = "urn:uuid:1.2.643.5.1.13.2.1.1.635";

        public const string OBSERVATION_LOINC = "urn:uuid:1.2.643.2.69.1.1.1.1";

        /// <summary>
        /// Код МЭС
        /// </summary>
        public const string MES = "urn:uuid:1.2.643.2.69.1.1.1.61";

        /// <summary>
        /// Код МКБ10
        /// </summary>
        public const string MKB10 = "urn:uuid:1.2.643.2.69.1.1.1.2";

        /// <summary>
        /// Код биоматериала
        /// </summary>
        public const string BIOMATERIAL = "urn:uuid:1.2.643.2.69.1.1.1.33";

        /// <summary>
        /// Тип контейнера
        /// </summary>
        public const string CONTAINER_TYPE = "urn:uuid:1.2.643.2.69.1.1.1.34";

        /// <summary>
        /// Тип случая обслуживания
        /// </summary>
        public const string ENCOUNTER_TYPE = "urn:uuid:1.2.643.2.69.1.1.1.35";

        /// <summary>
        /// Тип заболевания
        /// </summary>
        public const string CONDITION_CATEGORY = "urn:uuid:1.2.643.2.69.1.1.1.36";

        /// <summary>
        /// Код Observation
        /// </summary>
        public const string OBSERVATION_NAME = "urn:uuid:1.2.643.2.69.1.1.1.37";

        /// <summary>
        /// Код должности врача
        /// </summary>
        public const string PRACTITIONER_ROLE = "urn:uuid:1.2.643.5.1.13.2.1.1.607";

        /// <summary>
        /// Код специальности врача
        /// </summary>
        public const string PRACTITIONER_SPECIALITY = "urn:uuid:1.2.643.5.1.13.2.1.1.181";

        /// <summary>
        /// Тип страхового случая
        /// </summary>
        public const string COVERAGE_TYPE = "urn:uuid:1.2.643.2.69.1.1.1.48";

        /// <summary>
        /// Цель посещения
        /// </summary>
        public const string REASON_CODE = "urn:uuid:1.2.643.2.69.1.1.1.19";

        /// <summary>
        /// Отметка срочности
        /// </summary>
        public const string PRIORIRY = "urn:uuid:1.2.643.2.69.1.1.1.30";

        /// <summary>
        /// Код услуги заявки
        /// </summary>
        public const string DIAGNOSTIC_ORDER_CODE = "urn:uuid:1.2.643.2.69.1.1.1.31";

        /// <summary>
        /// Код источника финансирования
        /// </summary>
        public const string FINANCIAL = "urn:uuid:1.2.643.2.69.1.1.1.32";

        /// <summary>
        /// Номенклатура медицинских услуг, 1.2.643.5.1.13.2.1.1.473
        /// </summary>
        public const string DIAGNOSTIC_REPORT_CODE = "urn:uuid:1.2.643.2.69.1.1.1.31";

        #endregion

        #region Extensions

        /// <summary>
        /// Расширение "источник финансирования" для DiagnosticOrder
        /// </summary>
        public const string DIAGNOSTIC_ORDER_FINANCIAL_EXTENSION = "urn:uuid:1.2.643.2.69.1.100.1";

        /// <summary>
        /// Расширение "Данные о страховке" для DiagnosticOrder
        /// </summary>
        public const string DIAGNOSTIC_ORDER_INSURANCE_EXTENSION = "urn:uuid:1.2.643.2.69.1.100.2";

        #endregion
    }
}

