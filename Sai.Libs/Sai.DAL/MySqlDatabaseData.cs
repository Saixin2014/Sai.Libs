using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Instrumentation;
using System.Configuration;

namespace MySqlDal.Data
{
    /// <summary>
    /// Describes a <see cref="SqlDatabase"/> instance, aggregating information from a 
    /// <see cref="ConnectionStringSettings"/>.
    /// </summary>
    public class MySqlDatabaseData : DatabaseData
    {
        #region Public Methods
        public MySqlDatabaseData(ConnectionStringSettings connectionStringSettings, IConfigurationSource configurationSource)
            : base(connectionStringSettings, configurationSource)
        {
        }
        #endregion

        /// <summary>
        /// Creates a <see cref="TypeRegistration"/> instance describing the <see cref="SqlDatabase"/> represented by 
        /// this configuration object.
        /// </summary>
        /// <returns>A <see cref="TypeRegistration"/> instance describing a database.</returns>
        public override System.Collections.Generic.IEnumerable<Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel.TypeRegistration> GetRegistrations()
        {
            yield return new TypeRegistration<Database>(
                () => new MySqlDatabase(
                    ConnectionString,
                    Container.Resolved<IDataInstrumentationProvider>(Name)))
            {
                Name = Name,
                Lifetime = TypeRegistrationLifetime.Transient
            };
        }
    }
}
