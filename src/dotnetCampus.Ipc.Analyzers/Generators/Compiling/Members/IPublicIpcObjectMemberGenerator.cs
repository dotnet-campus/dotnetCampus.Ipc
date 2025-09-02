using dotnetCampus.Ipc.Generators.Models;

namespace dotnetCampus.Ipc.Generators.Compiling.Members;

internal interface IPublicIpcObjectProxyMemberGenerator
{
    MemberDeclarationSourceTextBuilder GenerateProxyMember(SourceTextBuilder builder);
}

internal interface IPublicIpcObjectShapeMemberGenerator
{
    MemberDeclarationSourceTextBuilder GenerateShapeMember(SourceTextBuilder builder);
}

internal interface IPublicIpcObjectJointMatchGenerator
{
    string GenerateJointMatch(SourceTextBuilder builder, string real);
}
