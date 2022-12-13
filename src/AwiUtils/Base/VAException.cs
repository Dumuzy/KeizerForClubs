using System;
using System.Runtime.Serialization;

namespace AwiUtils
{
    /// <summary> Diese Ausnahme ist die Basisklasse f�r alle unkritischen Fehler, die
    /// zum Client �bertragen werden sollen.  </summary>
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
        /// die innere Ausnahme, die diese Ausnahme ausgel�st hat.
        /// </summary>
        /// <param name="message">Die Fehlermeldung, in der die Ursache der
        /// Ausnahme erkl�rt wird.</param>
        /// <param name="innerException">Die Ausnahme, die Ursache der aktuellen
        /// Ausnahme ist. Wenn der <paramref name="innerException"/>-Parameter
        /// kein <see langword="null"/>-Verweis ist, wird die aktuelle Ausnahme
        /// in einem <see langword="catch"/>-Block ausgel�st, der die
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
        /// f�r die ausgel�ste Ausnahme enth�lt.</param>
        /// <param name="context">Die Kontextinformationen �ber die Quelle oder
        /// das Ziel.</param>
        protected VAException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary> NS f�r NoStack-Exception.  Diese Exception kann verwendet werden, wenn im Log der Stack der 
    /// Exception nicht gezeigt werden soll. Nur f�r's Managemen-Tool.</summary>
    public class NSException : VAException
    {
        public NSException(string msg) : base(msg) { }
    }

}
