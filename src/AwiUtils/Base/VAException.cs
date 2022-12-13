using System;
using System.Runtime.Serialization;

namespace AwiUtils
{
    /// <summary> Diese Ausnahme ist die Basisklasse für alle unkritischen Fehler, die
    /// zum Client übertragen werden sollen.  </summary>
    [Serializable]
    public class VAException : Exception
    {
        public VAException() { }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="VAException"/>
        /// -Klasse mit einer angegebenen Fehlermeldung.
        /// </summary>
        /// <param name="message">Die Meldung, in der der Fehler beschrieben
        /// wird.</param>
        public VAException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="VAException"/>
        /// -Klasse mit einer angegebenen Fehlermeldung und einem Verweis auf
        /// die innere Ausnahme, die diese Ausnahme ausgelöst hat.
        /// </summary>
        /// <param name="message">Die Fehlermeldung, in der die Ursache der
        /// Ausnahme erklärt wird.</param>
        /// <param name="innerException">Die Ausnahme, die Ursache der aktuellen
        /// Ausnahme ist. Wenn der <paramref name="innerException"/>-Parameter
        /// kein <see langword="null"/>-Verweis ist, wird die aktuelle Ausnahme
        /// in einem <see langword="catch"/>-Block ausgelöst, der die
        /// innere Ausnahme behandelt.</param>
        public VAException(string message,
            Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="VAException"/>
        /// -Klasse mit serialisierten Daten.
        /// </summary>
        /// <param name="info">Das Objekt, das die serialisierten Objektdaten
        /// für die ausgelöste Ausnahme enthält.</param>
        /// <param name="context">Die Kontextinformationen über die Quelle oder
        /// das Ziel.</param>
        protected VAException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary> NS für NoStack-Exception.  Diese Exception kann verwendet werden, wenn im Log der Stack der 
    /// Exception nicht gezeigt werden soll. Nur für's Managemen-Tool.</summary>
    public class NSException : VAException
    {
        public NSException(string msg) : base(msg) { }
    }

}
