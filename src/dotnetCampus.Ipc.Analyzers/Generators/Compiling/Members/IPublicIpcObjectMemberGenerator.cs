namespace dotnetCampus.Ipc.Generators.Compiling.Members;

internal interface IPublicIpcObjectProxyMemberGenerator
{
    string GenerateProxyMember();
}

internal interface IPublicIpcObjectShapeMemberGenerator
{
    string GenerateShapeMember();
}

internal interface IPublicIpcObjectJointMatchGenerator
{
    string GenerateJointMatch(string real);
}
