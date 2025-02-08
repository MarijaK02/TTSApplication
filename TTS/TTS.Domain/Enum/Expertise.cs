using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Enum
{
    public enum Expertise
    {
        [Display(Name = "Front-End Development")]
        FrontEndDevelopment,

        [Display(Name = "Back-End Development")]
        BackEndDevelopment,

        [Display(Name = "Full-Stack Development")]
        FullStackDevelopment,

        [Display(Name = "Mobile Development")]
        MobileDevelopment,

        [Display(Name = "DevOps")]
        DevOps,

        [Display(Name = "Data Engineering")]
        DataEngineering,

        [Display(Name = "Quality Assurance")]
        QualityAssurance,

        [Display(Name = "Cloud Engineering")]
        CloudEngineering,

        [Display(Name = "Cyber Security")]
        CyberSecurity,

        [Display(Name = "Embedded Systems")]
        EmbeddedSystems,

        // Specializations and Frameworks
        [Display(Name = "Game Development")]
        GameDevelopment,

        [Display(Name = "Machine Learning")]
        MachineLearning,

        [Display(Name = "Artificial Intelligence")]
        ArtificialIntelligence,

        [Display(Name = "Natural Language Processing")]
        NaturalLanguageProcessing,

        [Display(Name = "Data Science")]
        DataScience,

        [Display(Name = "Blockchain Development")]
        BlockchainDevelopment,

        [Display(Name = "Internet of Things (IoT)")]
        InternetOfThings,

        // Web Frameworks
        [Display(Name = "ASP.NET Core")]
        ASPNetCore,

        [Display(Name = "Django")]
        Django,

        [Display(Name = "Ruby on Rails")]
        RubyOnRails,

        [Display(Name = "Spring Framework")]
        SpringFramework,

        [Display(Name = "Node.js")]
        NodeJs,

        // Programming Languages
        [Display(Name = "C#")]
        CSharp,

        [Display(Name = "Java")]
        Java,

        [Display(Name = "Python")]
        Python,

        [Display(Name = "JavaScript")]
        JavaScript,

        [Display(Name = "TypeScript")]
        TypeScript,

        [Display(Name = "PHP")]
        PHP,

        [Display(Name = "Go")]
        Go,

        [Display(Name = "Swift")]
        Swift,

        [Display(Name = "Kotlin")]
        Kotlin,

        [Display(Name = "Rust")]
        Rust,

        // Tools and Technologies
        [Display(Name = "Docker (Containerization)")]
        ContainerizationDocker,

        [Display(Name = "Kubernetes (Container Orchestration)")]
        ContainerOrchestrationKubernetes,

        [Display(Name = "Continuous Integration/Continuous Deployment (CI/CD)")]
        CI_CD,

        [Display(Name = "Version Control (Git)")]
        VersionControlGit,

        [Display(Name = "Big Data")]
        BigData,

        [Display(Name = "Business Intelligence")]
        BusinessIntelligence,

        [Display(Name = "AWS Cloud")]
        CloudAWS,

        [Display(Name = "Azure Cloud")]
        CloudAzure,

        [Display(Name = "Google Cloud Platform (GCP)")]
        CloudGCP,

        // Database Technologies
        [Display(Name = "SQL Databases")]
        SQL,

        [Display(Name = "NoSQL Databases")]
        NoSQL,

        [Display(Name = "Database Administration")]
        DatabaseAdministration,

        [Display(Name = "Data Modeling")]
        DataModeling,

        // Soft Skills and Management
        [Display(Name = "Project Management")]
        ProjectManagement,

        [Display(Name = "Product Management")]
        ProductManagement,

        [Display(Name = "Agile Methodologies")]
        AgileMethodologies,

        [Display(Name = "Technical Writing")]
        TechnicalWriting,

        [Display(Name = "Team Leadership")]
        TeamLeadership,

        // UI/UX and Design
        [Display(Name = "UX Design")]
        UXDesign,

        [Display(Name = "UI Development")]
        UIDevelopment,

        [Display(Name = "Graphic Design")]
        GraphicDesign,

        // Miscellaneous
        [Display(Name = "Systems Architecture")]
        SystemsArchitecture,

        [Display(Name = "Networking")]
        Networking,

        [Display(Name = "IT Support")]
        ITSupport,

        [Display(Name = "Hardware Design")]
        HardwareDesign
    }
}
