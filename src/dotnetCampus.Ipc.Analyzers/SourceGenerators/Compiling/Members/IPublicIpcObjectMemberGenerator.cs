using dotnetCampus.Ipc.SourceGenerators.Models;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

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
