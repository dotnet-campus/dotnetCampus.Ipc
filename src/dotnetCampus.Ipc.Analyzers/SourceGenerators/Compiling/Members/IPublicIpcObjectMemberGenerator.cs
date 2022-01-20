namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal interface IPublicIpcObjectProxyMemberGenerator
{
    string GenerateProxyMember();
}

internal interface IPublicIpcObjectJointMatchGenerator
{
    string GenerateJointMatch(string real);
}
