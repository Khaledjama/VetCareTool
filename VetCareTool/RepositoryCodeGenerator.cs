﻿using Humanizer.Inflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCareTool
{
    public class RepositoryCodeGenerator
    {
        private string _projectPath { get; }
        public RepositoryCodeGenerator(string projectPath)
        {
            _projectPath = projectPath;
        }
        public void Execute()
        {
            string projectRootPath = _projectPath;
            Console.WriteLine("Enter entity name as singular:");
            string entityName = Console.ReadLine();
            if (String.IsNullOrEmpty(entityName))
            {
                Console.WriteLine("Entity name is Required");
                return;
            }

            string repositoryInterfaceName = "I" + entityName + "Repository";
            string repositoryImplementationName = entityName + "Repository";
            string pluralizedEntityName = Vocabularies.Default.Pluralize(entityName); // Adjust the pluralization logic as needed
                                                                                      //Entity Expected Path
            string entityExpectedPath = Path.Combine(_projectPath, "VetICare.Domain", "Entities", $"{entityName}.cs");
            HelperFun.CreateEntityIfNotExist(entityExpectedPath);

            string repositoryInterfaceCode = $@"
    using VetICare.Domain.Entities;
    using VetICare.Domain.Repositories.Interfaces.Generic;

    namespace VetICare.Domain.Repositories.Interfaces.Entities
    {{
        public interface {repositoryInterfaceName}:IEntityRepository<{entityName}> {{ }}
    }}";

            string repositoryImplementationCode = $@"
    using VetICare.Domain.Repositories.Interfaces.Entities;
    using VetICare.Domain.Entities;
    using VetICare.Persistence;

    namespace VetICare.Infrastructure.Repositories
    {{
        internal class {repositoryImplementationName} : EntityRepository<{entityName}>,{repositoryInterfaceName}
        {{
            public {repositoryImplementationName}(CareDbContext context) : base(context){{}}
        }}
    }}";

            string addInterfacePropertyCode = $"\t\t\t {repositoryInterfaceName} {pluralizedEntityName} {{ get;}} ";

            string addRepositoryPropertyCode = $"\t\t\t public {repositoryInterfaceName} {pluralizedEntityName} {{get;private set;}} ";

            string assignNewRepositoryCode = $"\t\t\t {pluralizedEntityName} = new {repositoryImplementationName}(_dbContext); ";

            string assignNewprivaterepositoryfields = $"\t\t\t private {repositoryImplementationName} _{pluralizedEntityName} ; ";
            //        public IAppointmentRepository Appointments => _appointmentsRepository ?? new AppointmentRepository(_dbContext);
            string assignNewrepositoryfunctions = $"\t\t\t public {repositoryInterfaceName} {pluralizedEntityName}=>  _{pluralizedEntityName} ?? new {repositoryImplementationName}(_dbContext); ";


            string repositoryInterfacePath = Path.Combine(projectRootPath, "VetICare.Domain", "Repositories", "Interfaces", "Entities", $"{repositoryInterfaceName}.cs");
            string repositoryImplementationPath = Path.Combine(projectRootPath, "VetICare.Infrastructure", "Repositories", $"{repositoryImplementationName}.cs");
            string unitOWorkInterfacePath = Path.Combine(projectRootPath, "VetICare.Domain", "Repositories", "Interfaces", "Generic", "IUnitOfWork.cs");
            string unitOfWorkImplementationPath = Path.Combine(projectRootPath, "VetICare.Infrastructure", "Repositories", "UnitOfWork.cs");

            // Write the repository interface code to a file
            if (!File.Exists(repositoryInterfacePath)) File.WriteAllText(repositoryInterfacePath, repositoryInterfaceCode);

            // Write the repository implementation code to a file
            if (!File.Exists(repositoryImplementationPath)) File.WriteAllText(repositoryImplementationPath, repositoryImplementationCode);



            // Adding code in IUnitOfWork.cs
            HelperFun.InsertCodeAfterLine(unitOWorkInterfacePath, @"^\s*public\s+interface\s+IUnitOfWork\s*:\s*IAsyncDisposable\s*$\n\s*\{", addInterfacePropertyCode);

            // Adding code in UnitOfWork.cs  ==>  in (#rgion private repository fields)
            HelperFun.InsertCodeAfterLine(unitOfWorkImplementationPath, @"^\s*#region\s+private\s+repository\s+fields\s*$", assignNewprivaterepositoryfields);

            // Adding code in UnitOfWork.cs  ==>  in (#rgion repository functions)
            HelperFun.InsertCodeAfterLine(unitOfWorkImplementationPath, @"^\s*#region\s+repository\s+functions\s*$", assignNewrepositoryfunctions);

            Console.WriteLine($"Repository code for {entityName} created successfully");

        }
    }
}
