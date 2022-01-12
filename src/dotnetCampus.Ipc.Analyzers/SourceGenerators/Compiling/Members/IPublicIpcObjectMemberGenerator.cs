namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal interface IPublicIpcObjectMemberGenerator
{
}

internal interface IPublicIpcObjectProxyMemberGenerator : IPublicIpcObjectMemberGenerator
{
    string GenerateProxyMember();
}

internal interface IPublicIpcObjectJointMatchGenerator : IPublicIpcObjectMemberGenerator
{
    string GenerateJointMatch(string real);
}
